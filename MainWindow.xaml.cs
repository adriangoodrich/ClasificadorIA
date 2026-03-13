using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace ClasificadorIA
{
    // Enum unificado a nivel de namespace (usado en MainWindow y ProgressWindow)
    public enum Idioma { Español, Inglés }

    public partial class MainWindow : Window
    {
        private string _carpetaSeleccionada = "";
        private List<string> _archivos = new List<string>();
        private Dictionary<string, List<string>> _categorias = new Dictionary<string, List<string>>();
        private string _promptGenerado = "";

        private Idioma _idiomaActual = Idioma.Español;

        // Modos de clasificación
        private readonly Dictionary<string, ModoInfo> _modos = new()
        {
            ["📁 Genérico"] = new("archivos de cualquier tipo", new[] { "Tema" }),
            ["🎵 Música"] = new("canciones", new[] { "Género", "Artista", "Época", "Álbum" }),
            ["🎬 Películas"] = new("películas", new[] { "Género", "Director", "Año", "Saga" }),
            ["📺 Series"] = new("series", new[] { "Género", "Cadena", "Año", "Saga" }),
            ["📚 Libros"] = new("libros", new[] { "Género", "Autor", "Año", "Editorial" })
        };

        // Traducciones de criterios
        private readonly Dictionary<string, Dictionary<Idioma, string>> _traduccionesCriterios = new()
        {
            ["Tema"] = new() { [Idioma.Español] = "Tema", [Idioma.Inglés] = "Topic" },
            ["Género"] = new() { [Idioma.Español] = "Género", [Idioma.Inglés] = "Genre" },
            ["Artista"] = new() { [Idioma.Español] = "Artista", [Idioma.Inglés] = "Artist" },
            ["Época"] = new() { [Idioma.Español] = "Época", [Idioma.Inglés] = "Era" },
            ["Álbum"] = new() { [Idioma.Español] = "Álbum", [Idioma.Inglés] = "Album" },
            ["Director"] = new() { [Idioma.Español] = "Director", [Idioma.Inglés] = "Director" },
            ["Año"] = new() { [Idioma.Español] = "Año", [Idioma.Inglés] = "Year" },
            ["Saga"] = new() { [Idioma.Español] = "Saga", [Idioma.Inglés] = "Saga" },
            ["Cadena"] = new() { [Idioma.Español] = "Cadena", [Idioma.Inglés] = "Network" },
            ["Autor"] = new() { [Idioma.Español] = "Autor", [Idioma.Inglés] = "Author" },
            ["Editorial"] = new() { [Idioma.Español] = "Editorial", [Idioma.Inglés] = "Publisher" }
        };

        // Textos de la interfaz
        private readonly Dictionary<string, Dictionary<Idioma, string>> _textosUI = new()
        {
            ["TituloPrincipal"] = new() { [Idioma.Español] = "📁 CLASIFICADOR IA DE ARCHIVOS", [Idioma.Inglés] = "📁 AI FILE CLASSIFIER" },
            ["SeleccionarCarpeta"] = new() { [Idioma.Español] = "📂 1. SELECCIONAR CARPETA", [Idioma.Inglés] = "📂 1. SELECT FOLDER" },
            ["Examinar"] = new() { [Idioma.Español] = "Examinar", [Idioma.Inglés] = "Browse" },
            ["ArchivosEncontrados"] = new() { [Idioma.Español] = "archivos encontrados", [Idioma.Inglés] = "files found" },
            ["NingunaCarpeta"] = new() { [Idioma.Español] = "Ninguna carpeta seleccionada", [Idioma.Inglés] = "No folder selected" },
            ["Modo"] = new() { [Idioma.Español] = "🎯 MODO", [Idioma.Inglés] = "🎯 MODE" },
            ["Criterio"] = new() { [Idioma.Español] = "🔍 CRITERIO", [Idioma.Inglés] = "🔍 CRITERION" },
            ["Profundidad"] = new() { [Idioma.Español] = "🎚️ PROFUNDIDAD", [Idioma.Inglés] = "🎚️ DEPTH" },
            ["Categorias5"] = new() { [Idioma.Español] = "5 categorías", [Idioma.Inglés] = "5 categories" },
            ["Categorias10"] = new() { [Idioma.Español] = "10 categorías", [Idioma.Inglés] = "10 categories" },
            ["Categorias15"] = new() { [Idioma.Español] = "15 categorías", [Idioma.Inglés] = "15 categories" },
            ["Prompt"] = new() { [Idioma.Español] = "🤖 4. PROMPT PARA IA", [Idioma.Inglés] = "🤖 4. AI PROMPT" },
            ["Copiar"] = new() { [Idioma.Español] = "📋 Copiar", [Idioma.Inglés] = "📋 Copy" },
            ["Guardar"] = new() { [Idioma.Español] = "💾 Guardar", [Idioma.Inglés] = "💾 Save" },
            ["Respuesta"] = new() { [Idioma.Español] = "📥 5. RESPUESTA DE LA IA (categorías)", [Idioma.Inglés] = "📥 5. AI RESPONSE (categories)" },
            ["Cargar"] = new() { [Idioma.Español] = "📂 Cargar", [Idioma.Inglés] = "📂 Load" },
            ["Pegar"] = new() { [Idioma.Español] = "📋 Pegar", [Idioma.Inglés] = "📋 Paste" },
            ["CopiarArchivos"] = new() { [Idioma.Español] = "📋 Copiar archivos", [Idioma.Inglés] = "📋 Copy files" },
            ["MoverArchivos"] = new() { [Idioma.Español] = "✂️ Mover archivos", [Idioma.Inglés] = "✂️ Move files" },
            ["Iniciar"] = new() { [Idioma.Español] = "🚀 INICIAR", [Idioma.Inglés] = "🚀 START" },
            ["Cancelar"] = new() { [Idioma.Español] = "❌ CANCELAR", [Idioma.Inglés] = "❌ CANCEL" },
            ["CategoriasDetectadas"] = new() { [Idioma.Español] = "📊 CATEGORÍAS DETECTADAS", [Idioma.Inglés] = "📊 DETECTED CATEGORIES" },
            ["Total"] = new() { [Idioma.Español] = "📊 Total:", [Idioma.Inglés] = "📊 Total:" },
            ["Archivos"] = new() { [Idioma.Español] = "archivos", [Idioma.Inglés] = "files" },
            ["ProcesandoArchivos"] = new() { [Idioma.Español] = "Procesando archivos...", [Idioma.Inglés] = "Processing files..." },
            ["CancelarProgress"] = new() { [Idioma.Español] = "Cancelar", [Idioma.Inglés] = "Cancel" }
        };

        private string _promptTemplateEs = @"Eres un experto en clasificación de {0}.
Clasifica estos archivos por **{1}** según su nombre.
Intenta generar aproximadamente {2} categorías.

Archivos:
{3}

Devuelve SOLO este JSON:
{{
  ""categorias"": {{
    ""Categoría1"": [""archivo1"", ""archivo2""],
    ""Categoría2"": [""archivo3""]
  }}
}}";

        private string _promptTemplateEn = @"You are an expert in classifying {0}.
Classify these files by **{1}** based on their name.
Try to generate approximately {2} categories.

Files:
{3}

Return ONLY this JSON:
{{
  ""categories"": {{
    ""Category1"": [""file1"", ""file2""],
    ""Category2"": [""file3""]
  }}
}}";

        private CancellationTokenSource _cts;

        public MainWindow()
        {
            InitializeComponent();
            CargarCombos();
            AplicarIdioma();
        }

        private void CargarCombos()
        {
            try
            {
                ComboModo.ItemsSource = _modos.Keys.ToList();
                ComboModo.SelectedIndex = 0;

                if (ComboProfundidad != null)
                {
                    ComboProfundidad.Items.Clear();
                    ComboProfundidad.Items.Add(new ComboBoxItem { Content = _textosUI["Categorias5"][_idiomaActual], Tag = "5" });
                    ComboProfundidad.Items.Add(new ComboBoxItem { Content = _textosUI["Categorias10"][_idiomaActual], Tag = "10" });
                    ComboProfundidad.Items.Add(new ComboBoxItem { Content = _textosUI["Categorias15"][_idiomaActual], Tag = "15" });
                    ComboProfundidad.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar combos", ex);
            }
        }

        private void ComboIdioma_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboIdioma.SelectedItem is ComboBoxItem item && item.Tag is string tag)
            {
                _idiomaActual = tag == "es" ? Idioma.Español : Idioma.Inglés;
                AplicarIdioma();
                if (_archivos.Count > 0) GenerarPrompt();
            }
        }

        private void AplicarIdioma()
        {
            if (TituloPrincipal != null) TituloPrincipal.Text = _textosUI["TituloPrincipal"][_idiomaActual];
            if (LblSeleccionarCarpeta != null) LblSeleccionarCarpeta.Text = _textosUI["SeleccionarCarpeta"][_idiomaActual];
            if (BtnExaminar != null) BtnExaminar.Content = _textosUI["Examinar"][_idiomaActual];
            if (TxtRuta != null) TxtRuta.Text = _textosUI["NingunaCarpeta"][_idiomaActual];
            if (LblModo != null) LblModo.Text = _textosUI["Modo"][_idiomaActual];
            if (LblCriterio != null) LblCriterio.Text = _textosUI["Criterio"][_idiomaActual];
            if (LblProfundidad != null) LblProfundidad.Text = _textosUI["Profundidad"][_idiomaActual];
            if (LblPrompt != null) LblPrompt.Text = _textosUI["Prompt"][_idiomaActual];
            if (BtnCopiarPrompt != null) BtnCopiarPrompt.Content = _textosUI["Copiar"][_idiomaActual];
            if (BtnGuardarPrompt != null) BtnGuardarPrompt.Content = _textosUI["Guardar"][_idiomaActual];
            if (LblRespuesta != null) LblRespuesta.Text = _textosUI["Respuesta"][_idiomaActual];
            if (BtnCargar != null) BtnCargar.Content = _textosUI["Cargar"][_idiomaActual];
            if (BtnPegar != null) BtnPegar.Content = _textosUI["Pegar"][_idiomaActual];
            if (RbCopiar != null) RbCopiar.Content = _textosUI["CopiarArchivos"][_idiomaActual];
            if (RbMover != null) RbMover.Content = _textosUI["MoverArchivos"][_idiomaActual];
            if (BtnIniciar != null) BtnIniciar.Content = _textosUI["Iniciar"][_idiomaActual];
            if (BtnCancelar != null) BtnCancelar.Content = _textosUI["Cancelar"][_idiomaActual];

            if (ComboProfundidad != null && ComboProfundidad.Items.Count >= 3)
            {
                ((ComboBoxItem)ComboProfundidad.Items[0]).Content = _textosUI["Categorias5"][_idiomaActual];
                ((ComboBoxItem)ComboProfundidad.Items[1]).Content = _textosUI["Categorias10"][_idiomaActual];
                ((ComboBoxItem)ComboProfundidad.Items[2]).Content = _textosUI["Categorias15"][_idiomaActual];
            }

            if (TxtArchivos != null && _archivos.Count > 0)
                TxtArchivos.Text = $"📊 {_archivos.Count} {_textosUI["ArchivosEncontrados"][_idiomaActual]}";
            else if (TxtArchivos != null)
                TxtArchivos.Text = $"0 {_textosUI["ArchivosEncontrados"][_idiomaActual]}";
        }

        private void ComboModo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboModo.SelectedItem == null) return;
                var modo = _modos[ComboModo.SelectedItem.ToString()];
                var criteriosVisibles = modo.Criterios.Select(c => _traduccionesCriterios[c][_idiomaActual]).ToList();
                ComboCriterio.ItemsSource = criteriosVisibles;
                ComboCriterio.SelectedIndex = 0;
                if (_archivos.Count > 0) GenerarPrompt();
            }
            catch (Exception ex)
            {
                MostrarError("Error al cambiar modo", ex);
            }
        }

        private void ComboCriterio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_archivos.Count > 0) GenerarPrompt();
            }
            catch (Exception ex)
            {
                MostrarError("Error al cambiar criterio", ex);
            }
        }

        private void ComboProfundidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_archivos.Count > 0) GenerarPrompt();
        }

        private int GetProfundidad()
        {
            if (ComboProfundidad.SelectedItem is ComboBoxItem item && item.Tag is string tag)
                return int.Parse(tag);
            return 5;
        }

        private void BtnExaminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFolderDialog { Title = _idiomaActual == Idioma.Español ? "Selecciona la carpeta a organizar" : "Select folder to organize" };
                if (dialog.ShowDialog() == true)
                {
                    _carpetaSeleccionada = dialog.FolderName;
                    TxtRuta.Text = _carpetaSeleccionada;
                    CargarArchivos();
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error al seleccionar carpeta", ex);
            }
        }

        private void CargarArchivos()
        {
            try
            {
                if (!Directory.Exists(_carpetaSeleccionada))
                {
                    MessageBox.Show(_idiomaActual == Idioma.Español ? "La carpeta no existe." : "Folder does not exist.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _archivos = Directory.GetFiles(_carpetaSeleccionada)
                    .Select(Path.GetFileName)
                    .Where(f => !f.EndsWith(".exe") && !f.EndsWith(".dll") &&
                               !f.EndsWith(".ps1") && !f.EndsWith(".bat") &&
                               !f.EndsWith(".cs") && !f.EndsWith(".csproj"))
                    .ToList();

                TxtArchivos.Text = $"📊 {_archivos.Count} {_textosUI["ArchivosEncontrados"][_idiomaActual]}";

                if (_archivos.Count == 0)
                {
                    MessageBox.Show(_idiomaActual == Idioma.Español ? "No hay archivos para clasificar." : "No files to classify.",
                        "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (ComboModo.SelectedItem != null && ComboCriterio.SelectedItem != null)
                    GenerarPrompt();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(_idiomaActual == Idioma.Español ? "Sin permisos. Ejecuta como administrador." : "No permissions. Run as administrator.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar archivos", ex);
            }
        }

        private void GenerarPrompt()
        {
            try
            {
                if (ComboModo.SelectedItem == null || ComboCriterio.SelectedItem == null) return;

                var modo = _modos[ComboModo.SelectedItem.ToString()];
                string criterioSeleccionado = ComboCriterio.SelectedItem.ToString();
                string criterioOriginal = _traduccionesCriterios.FirstOrDefault(x => x.Value[_idiomaActual] == criterioSeleccionado).Key;
                if (string.IsNullOrEmpty(criterioOriginal)) criterioOriginal = criterioSeleccionado;

                int profundidad = GetProfundidad();
                string archivosTexto = string.Join("\n", _archivos.Select(a => $"- {a}"));

                if (_idiomaActual == Idioma.Español)
                    _promptGenerado = string.Format(_promptTemplateEs, modo.Descripcion, criterioOriginal.ToLower(), profundidad, archivosTexto);
                else
                    _promptGenerado = string.Format(_promptTemplateEn, modo.Descripcion, criterioOriginal.ToLower(), profundidad, archivosTexto);

                TxtPromptPreview.Text = _promptGenerado;
            }
            catch (Exception ex)
            {
                MostrarError("Error al generar prompt", ex);
            }
        }

        private void BtnCopiarPrompt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_promptGenerado))
                {
                    MessageBox.Show(_idiomaActual == Idioma.Español ? "No hay prompt." : "No prompt.");
                    return;
                }
                Clipboard.SetText(_promptGenerado);
                MessageBox.Show(_idiomaActual == Idioma.Español ? "✅ Prompt copiado" : "✅ Prompt copied",
                    "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MostrarError("Error al copiar", ex);
            }
        }

        private void BtnGuardarPrompt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_promptGenerado))
                {
                    MessageBox.Show(_idiomaActual == Idioma.Español ? "No hay prompt." : "No prompt.");
                    return;
                }
                var dialog = new SaveFileDialog { Filter = "Texto|*.txt", FileName = "prompt.txt" };
                if (dialog.ShowDialog() == true)
                {
                    File.WriteAllText(dialog.FileName, _promptGenerado);
                    MessageBox.Show(_idiomaActual == Idioma.Español ? "✅ Guardado" : "✅ Saved", "Listo");
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error al guardar", ex);
            }
        }

        private void BtnPegarRespuesta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText()) ProcesarRespuesta(Clipboard.GetText());
                else MessageBox.Show(_idiomaActual == Idioma.Español ? "No hay texto en portapapeles." : "No text in clipboard.");
            }
            catch (Exception ex)
            {
                MostrarError("Error al pegar", ex);
            }
        }

        private void BtnCargarRespuesta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog { Filter = "Texto|*.txt;*.json" };
                if (dialog.ShowDialog() == true)
                    ProcesarRespuesta(File.ReadAllText(dialog.FileName));
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar archivo", ex);
            }
        }

        private string NormalizarNombre(string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) return "";
            return nombre.Trim().ToUpperInvariant();
        }

        private void ProcesarRespuesta(string contenido)
        {
            try
            {
                int inicio = contenido.IndexOf('{');
                int fin = contenido.LastIndexOf('}') + 1;
                if (inicio < 0 || fin <= inicio) throw new Exception(_idiomaActual == Idioma.Español ? "JSON no encontrado" : "JSON not found");

                string jsonStr = contenido.Substring(inicio, fin - inicio);
                using var doc = JsonDocument.Parse(jsonStr);

                string key = _idiomaActual == Idioma.Español ? "categorias" : "categories";
                if (!doc.RootElement.TryGetProperty(key, out JsonElement cats))
                {
                    key = _idiomaActual == Idioma.Español ? "categories" : "categorias";
                    if (!doc.RootElement.TryGetProperty(key, out cats))
                    {
                        MessageBox.Show(_idiomaActual == Idioma.Español ? "El JSON no contiene 'categorias'." : "JSON does not contain 'categories'.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                _categorias.Clear();

                var nombresRealesNormalizados = _archivos.ToDictionary(
                    a => NormalizarNombre(a),
                    a => a,
                    StringComparer.OrdinalIgnoreCase
                );

                foreach (var prop in cats.EnumerateObject())
                {
                    string cat = LimpiarNombre(prop.Name);
                    var archivosJson = prop.Value.EnumerateArray()
                        .Select(x => x.GetString())
                        .Where(x => !string.IsNullOrEmpty(x))
                        .ToList();

                    var archivosReales = new List<string>();
                    foreach (var nombreJson in archivosJson)
                    {
                        string nombreNormalizado = NormalizarNombre(nombreJson);
                        if (nombresRealesNormalizados.TryGetValue(nombreNormalizado, out string real))
                        {
                            archivosReales.Add(real);
                        }
                        else
                        {
                            Debug.WriteLine($"Nombre no encontrado: {nombreJson}");
                        }
                    }

                    if (archivosReales.Any())
                        _categorias[cat] = archivosReales;
                }

                if (_categorias.Count == 0)
                {
                    MessageBox.Show(_idiomaActual == Idioma.Español ? "No se encontraron categorías válidas." : "No valid categories found.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MostrarResultados();
                BtnIniciar.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MostrarError("Error al procesar respuesta", ex);
            }
        }

        private void MostrarResultados()
        {
            try
            {
                PanelCategorias.Children.Clear();
                if (_categorias.Count == 0)
                {
                    PanelCategorias.Children.Add(new TextBlock { Text = _idiomaActual == Idioma.Español ? "No hay categorías." : "No categories.", Margin = new Thickness(10) });
                    return;
                }

                PanelCategorias.Children.Add(new TextBlock
                {
                    Text = _textosUI["CategoriasDetectadas"][_idiomaActual],
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                int total = 0;
                foreach (var cat in _categorias.OrderByDescending(c => c.Value.Count))
                {
                    total += cat.Value.Count;
                    var border = new Border { Background = Brushes.LightGray, CornerRadius = new CornerRadius(5), Padding = new Thickness(8), Margin = new Thickness(0, 0, 0, 8) };
                    var stack = new StackPanel();
                    stack.Children.Add(new TextBlock { Text = $"📁 {cat.Key} ({cat.Value.Count})", FontWeight = FontWeights.Bold });
                    foreach (var a in cat.Value.Take(5))
                        stack.Children.Add(new TextBlock { Text = $"  • {a}", Margin = new Thickness(10, 1, 0, 1) });
                    if (cat.Value.Count > 5)
                        stack.Children.Add(new TextBlock { Text = $"  ... y {cat.Value.Count - 5} más", FontStyle = FontStyles.Italic });
                    border.Child = stack;
                    PanelCategorias.Children.Add(border);
                }
                PanelCategorias.Children.Add(new TextBlock
                {
                    Text = $"{_textosUI["Total"][_idiomaActual]} {total} {_textosUI["Archivos"][_idiomaActual]}",
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                });
            }
            catch (Exception ex)
            {
                MostrarError("Error al mostrar resultados", ex);
            }
        }

        private async void BtnIniciar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_carpetaSeleccionada) || _categorias.Count == 0)
                {
                    MessageBox.Show(_idiomaActual == Idioma.Español ? "Selecciona carpeta y carga una respuesta." : "Select folder and load a response.");
                    return;
                }

                bool copiar = RbCopiar.IsChecked == true;
                if (MessageBox.Show(
                    _idiomaActual == Idioma.Español ? $"¿{(copiar ? "COPIAR" : "MOVER")} archivos?" : $"{(copiar ? "COPY" : "MOVE")} files?",
                    _idiomaActual == Idioma.Español ? "Confirmar" : "Confirm",
                    MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    return;

                var todosArchivos = _categorias.SelectMany(c => c.Value).ToList();
                var archivosReales = Directory.GetFiles(_carpetaSeleccionada).Select(Path.GetFileName).ToHashSet(StringComparer.OrdinalIgnoreCase);
                var noEncontrados = todosArchivos.Where(a => !archivosReales.Contains(a)).ToList();

                if (noEncontrados.Any())
                {
                    string msg = _idiomaActual == Idioma.Español ? "Los siguientes archivos no se encontraron en la carpeta:\n" : "The following files were not found in the folder:\n";
                    msg += string.Join("\n", noEncontrados.Take(10));
                    if (noEncontrados.Count > 10) msg += $"\n... y {noEncontrados.Count - 10} más";
                    MessageBox.Show(msg, _idiomaActual == Idioma.Español ? "Archivos no encontrados" : "Files not found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int total = todosArchivos.Count;
                int procesados = 0;
                int errores = 0;

                var progressWindow = new ProgressWindow();
                progressWindow.Owner = this;
                progressWindow.SetIdioma(_idiomaActual, _textosUI["ProcesandoArchivos"][_idiomaActual], _textosUI["CancelarProgress"][_idiomaActual]);
                _cts = new CancellationTokenSource();

                progressWindow.Show();

                await Task.Run(() =>
                {
                    foreach (var cat in _categorias)
                    {
                        if (_cts.Token.IsCancellationRequested) break;

                        string dir = Path.Combine(_carpetaSeleccionada, LimpiarNombre(cat.Key));
                        try { Directory.CreateDirectory(dir); } catch { }

                        foreach (string arch in cat.Value)
                        {
                            if (_cts.Token.IsCancellationRequested) break;

                            try
                            {
                                string src = Path.Combine(_carpetaSeleccionada, arch);
                                string dst = Path.Combine(dir, arch);

                                if (File.Exists(src))
                                {
                                    if (copiar) File.Copy(src, dst, true);
                                    else File.Move(src, dst, true);
                                    procesados++;
                                }
                                else
                                {
                                    errores++;
                                }
                            }
                            catch { errores++; }

                            int porcentaje = (procesados * 100) / total;
                            if (porcentaje % 5 == 0)
                                progressWindow.ActualizarProgreso(porcentaje);
                        }
                    }
                }, _cts.Token);

                progressWindow.Close();

                string mensaje = $"{(copiar ? "✅ Copia completada" : "✅ Movimiento completado")}\n\n{(_idiomaActual == Idioma.Español ? "Procesados: " : "Processed: ")}{procesados}\n{(_idiomaActual == Idioma.Español ? "Errores: " : "Errors: ")}{errores}";
                MessageBox.Show(mensaje, _idiomaActual == Idioma.Español ? "Resultado" : "Result", MessageBoxButton.OK, errores > 0 ? MessageBoxImage.Warning : MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MostrarError("Error al iniciar", ex);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            _carpetaSeleccionada = "";
            _archivos.Clear();
            _categorias.Clear();
            _promptGenerado = "";

            TxtRuta.Text = _textosUI["NingunaCarpeta"][_idiomaActual];
            TxtArchivos.Text = $"0 {_textosUI["ArchivosEncontrados"][_idiomaActual]}";
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

        private void MostrarError(string contexto, Exception ex)
        {
            MessageBox.Show($"{(_idiomaActual == Idioma.Español ? "Error en " : "Error in ")}{contexto}:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public record ModoInfo(string Descripcion, string[] Criterios);
}