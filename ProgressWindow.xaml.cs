using System;
using System.Windows;

namespace ClasificadorIA
{
    public partial class ProgressWindow : Window
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        // Método para establecer el idioma desde la ventana principal
        public void SetIdioma(Idioma idioma, string textoProcesando, string textoCancelar)
        {
            TxtMensaje.Text = textoProcesando;
            BtnCancelar.Content = textoCancelar;
            this.Title = idioma == Idioma.Español ? "Progreso" : "Progress";
        }

        public void ActualizarProgreso(int porcentaje)
        {
            Dispatcher.Invoke(() => ProgressBar.Value = porcentaje);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}