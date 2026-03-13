using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClasificadorIA
{
    public class Resources : INotifyPropertyChanged
    {
        private static Resources _instance;
        public static Resources Instance => _instance ??= new Resources();

        private string _currentLanguage = "es";
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    OnPropertyChanged();
                    // Actualizar todas las propiedades de texto
                    OnPropertyChanged(nameof(Title));
                    OnPropertyChanged(nameof(SelectFolder));
                    OnPropertyChanged(nameof(NoFolderSelected));
                    OnPropertyChanged(nameof(Browse));
                    OnPropertyChanged(nameof(FilesFound));
                    OnPropertyChanged(nameof(Mode));
                    OnPropertyChanged(nameof(Criterion));
                    OnPropertyChanged(nameof(Depth));
                    OnPropertyChanged(nameof(Prompt));
                    OnPropertyChanged(nameof(Copy));
                    OnPropertyChanged(nameof(Save));
                    OnPropertyChanged(nameof(AIResponse));
                    OnPropertyChanged(nameof(Load));
                    OnPropertyChanged(nameof(Paste));
                    OnPropertyChanged(nameof(CopyFiles));
                    OnPropertyChanged(nameof(MoveFiles));
                    OnPropertyChanged(nameof(Start));
                    OnPropertyChanged(nameof(Cancel));
                }
            }
        }

        // Textos en español e inglés
        public string Title => CurrentLanguage == "es" ? "📁 CLASIFICADOR IA DE ARCHIVOS" : "📁 AI FILE CLASSIFIER";
        public string SelectFolder => CurrentLanguage == "es" ? "📂 1. SELECCIONAR CARPETA" : "📂 1. SELECT FOLDER";
        public string NoFolderSelected => CurrentLanguage == "es" ? "Ninguna carpeta seleccionada" : "No folder selected";
        public string Browse => CurrentLanguage == "es" ? "Examinar" : "Browse";
        public string FilesFound => CurrentLanguage == "es" ? "0 archivos encontrados" : "0 files found";
        public string Mode => CurrentLanguage == "es" ? "🎯 MODO" : "🎯 MODE";
        public string Criterion => CurrentLanguage == "es" ? "🔍 CRITERIO" : "🔍 CRITERION";
        public string Depth => CurrentLanguage == "es" ? "🎚️ PROFUNDIDAD" : "🎚️ DEPTH";
        public string Prompt => CurrentLanguage == "es" ? "🤖 4. PROMPT PARA IA" : "🤖 4. AI PROMPT";
        public string Copy => CurrentLanguage == "es" ? "📋 Copiar" : "📋 Copy";
        public string Save => CurrentLanguage == "es" ? "💾 Guardar" : "💾 Save";
        public string AIResponse => CurrentLanguage == "es" ? "📥 5. RESPUESTA DE LA IA (categorías)" : "📥 5. AI RESPONSE (categories)";
        public string Load => CurrentLanguage == "es" ? "📂 Cargar" : "📂 Load";
        public string Paste => CurrentLanguage == "es" ? "📋 Pegar" : "📋 Paste";
        public string CopyFiles => CurrentLanguage == "es" ? "📋 Copiar archivos" : "📋 Copy files";
        public string MoveFiles => CurrentLanguage == "es" ? "✂️ Mover archivos" : "✂️ Move files";
        public string Start => CurrentLanguage == "es" ? "🚀 INICIAR" : "🚀 START";
        public string Cancel => CurrentLanguage == "es" ? "❌ CANCELAR" : "❌ CANCEL";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}