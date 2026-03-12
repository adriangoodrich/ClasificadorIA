using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace ClasificadorIA
{
    public partial class MainWindow : Window
    {
        private string _carpetaSeleccionada = "";
        private List<string> _archivos = new List<string>();
        private Dictionary<string, List<string>> _categorias = new Dictionary<string, List<string>>();
        private string _promptGenerado = "";

        // Modos de clasificación
        private readonly Dictionary<string, ModoInfo> _modos = new()
        {
            ["📁 Genérico"] = new("archivos de cualquier tipo", new[] { "Tema" }),
            ["🎵 Música"] = new("canciones", new[] { "Género", "Artista", "Época", "Álbum" }),
            ["🎬 Películas"] = new("películas", new[] { "Género", "Director", "Año", "Saga" }),
            ["📺 Series"] = new("series", new[] { "Género", "Cadena", "Año", "Saga" }),
            ["📚 Libros"] = new("libros", new[] { "Género", "Autor", "Año", "Editorial" })
        };

        public MainWindow()
        {
            InitializeComponent();
            CargarCombos();
        }

        private void CargarCombos()
        {
            ComboModo.ItemsSource = _modos.Keys.ToList();
            ComboModo.SelectedIndex = 0;
        }

        private void ComboModo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboModo.SelectedItem == null) return;

            var modo = _modos[ComboModo.SelectedItem.ToString()];
            ComboCriterio.ItemsSource = modo.Criterios;
            ComboCriterio.SelectedIndex = 0;

            if (_archivos.Count > 0) GenerarPrompt();
        }

        private void ComboCriterio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_archivos.Count > 0) GenerarPrompt();
        }

        private int GetProfundidad()
        {
            if (RbProfundidad15.IsChecked == true) return 15;
            if (RbProfundidad10.IsChecked == true) return 10;
            return 5; // por defecto
        }

        private void BtnExaminar_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog { Title = "Selecciona carpeta" };
            if (dialog.ShowDialog() == true)
            {
                _carpetaSeleccionada = dialog.FolderName;
                TxtRuta.Text = _carpetaSeleccionada;
                CargarArchivos();
            }
        }

        private void CargarArchivos()
        {
            try
            {
                _archivos = Directory.GetFiles(_carpetaSeleccionada)
                    .Select(Path.GetFileName)
                    .Where(f => !f.EndsWith(".exe") && !f.EndsWith(".dll") &&
                               !f.EndsWith(".ps1") && !f.EndsWith(".bat"))
                    .ToList();

                TxtArchivos.Text = $"📊 {_archivos.Count} archivos encontrados";

                if (_archivos.Count > 0)
                    GenerarPrompt();
                else
                    MessageBox.Show("No hay archivos para clasificar.", "Aviso");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void GenerarPrompt()
        {
            if (ComboModo.SelectedItem == null || ComboCriterio.SelectedItem == null) return;

            var modo = _modos[ComboModo.SelectedItem.ToString()];
            var criterio = ComboCriterio.SelectedItem.ToString();
            int profundidad = GetProfundidad();

            var sb = new StringBuilder();
            sb.AppendLine($"Eres un experto en clasificación de {modo.Descripcion}.");
            sb.AppendLine($"Clasifica estos archivos por **{criterio.ToLower()}** según su nombre.");
            sb.AppendLine($"Intenta generar aproximadamente {profundidad} categorías.");
            sb.AppendLine("\nArchivos:");
            foreach (var a in _archivos) sb.AppendLine($"- {a}");
            sb.AppendLine("\nDevuelve SOLO este JSON:");
            sb.AppendLine("{");
            sb.AppendLine("  \"categorias\": {");
            sb.AppendLine("    \"Categoría1\": [\"archivo1\", \"archivo2\"],");
            sb.AppendLine("    \"Categoría2\": [\"archivo3\"]");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            _promptGenerado = sb.ToString();
            TxtPromptPreview.Text = _promptGenerado;
        }

        private void BtnCopiarPrompt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(_promptGenerado);
                MessageBox.Show("✅ Prompt copiado", "Listo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void BtnGuardarPrompt_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Archivo de texto|*.txt",
                FileName = "prompt.txt"
            };
            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, _promptGenerado);
                MessageBox.Show("✅ Guardado", "Listo");
            }
        }

        private void BtnPegarRespuesta_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
                ProcesarRespuesta(Clipboard.GetText());
            else
                MessageBox.Show("No hay texto en el portapapeles.");
        }

        private void BtnCargarRespuesta_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Texto|*.txt;*.json" };
            if (dialog.ShowDialog() == true)
                ProcesarRespuesta(File.ReadAllText(dialog.FileName));
        }

        private void ProcesarRespuesta(string contenido)
        {
            try
            {
                int inicio = contenido.IndexOf('{');
                int fin = contenido.LastIndexOf('}') + 1;

                if (inicio < 0 || fin <= inicio)
                    throw new Exception("No se encontró JSON válido");

                string jsonStr = contenido.Substring(inicio, fin - inicio);
                using var doc = JsonDocument.Parse(jsonStr);

                if (doc.RootElement.TryGetProperty("categorias", out var cats))
                {
                    _categorias.Clear();
                    foreach (var prop in cats.EnumerateObject())
                    {
                        string cat = LimpiarNombre(prop.Name);
                        var archivos = prop.Value.EnumerateArray()
                            .Select(x => x.GetString())
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList();

                        if (archivos.Any())
                            _categorias[cat] = archivos;
                    }

                    MostrarResultados();
                    BtnIniciar.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void MostrarResultados()
        {
            PanelCategorias.Children.Clear();

            if (_categorias.Count == 0)
            {
                PanelCategorias.Children.Add(new TextBlock
                {
                    Text = "No hay categorías",
                    Margin = new Thickness(10)
                });
                return;
            }

            int total = 0;
            foreach (var cat in _categorias.OrderByDescending(c => c.Value.Count))
            {
                total += cat.Value.Count;

                var border = new Border
                {
                    Background = new SolidColorBrush(Colors.LightGray),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(8),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                var stack = new StackPanel();
                stack.Children.Add(new TextBlock
                {
                    Text = $"📁 {cat.Key} ({cat.Value.Count})",
                    FontWeight = FontWeights.Bold
                });

                foreach (var a in cat.Value.Take(5))
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"  • {a}",
                        Margin = new Thickness(10, 1, 0, 1)
                    });

                if (cat.Value.Count > 5)
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"  ... y {cat.Value.Count - 5} más",
                        FontStyle = FontStyles.Italic
                    });

                border.Child = stack;
                PanelCategorias.Children.Add(border);
            }

            PanelCategorias.Children.Add(new TextBlock
            {
                Text = $"📊 Total: {total} archivos",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            });
        }

        private async void BtnIniciar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_carpetaSeleccionada) || _categorias.Count == 0)
            {
                MessageBox.Show("Selecciona carpeta y carga respuesta.");
                return;
            }

            bool copiar = RbCopiar.IsChecked == true;
            if (MessageBox.Show($"¿{(copiar ? "COPIAR" : "MOVER")} archivos?", "Confirmar",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            MostrarProgreso(true, $"{(copiar ? "Copiando" : "Moviendo")}...");
            BtnIniciar.IsEnabled = false;

            int ok = 0, err = 0;

            foreach (var cat in _categorias)
            {
                string dir = Path.Combine(_carpetaSeleccionada, LimpiarNombre(cat.Key));
                Directory.CreateDirectory(dir);

                foreach (string arch in cat.Value)
                {
                    try
                    {
                        string src = Path.Combine(_carpetaSeleccionada, arch);
                        string dst = Path.Combine(dir, arch);

                        if (File.Exists(src))
                        {
                            if (copiar) File.Copy(src, dst, true);
                            else File.Move(src, dst, true);
                            ok++;
                        }
                        else err++;
                    }
                    catch { err++; }
                }
            }

            MostrarProgreso(false);
            MessageBox.Show($"✅ Completado\nOK: {ok}\nErrores: {err}");
            BtnIniciar.IsEnabled = true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            _carpetaSeleccionada = "";
            _archivos.Clear();
            _categorias.Clear();
            TxtRuta.Text = "Ninguna carpeta";
            TxtArchivos.Text = "0 archivos";
            TxtPromptPreview.Text = "";
            PanelCategorias.Children.Clear();
            BtnIniciar.IsEnabled = false;
        }

        private string LimpiarNombre(string n)
        {
            if (string.IsNullOrEmpty(n)) return "SinCategoria";
            foreach (char c in Path.GetInvalidFileNameChars())
                n = n.Replace(c, '_');
            return n.Length > 50 ? n[..50] : n.Trim();
        }

        private void MostrarProgreso(bool visible, string msg = "")
        {
            ProgressBar.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            TxtProgreso.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            TxtProgreso.Text = msg;
        }
    }

    public record ModoInfo(string Descripcion, string[] Criterios);
}