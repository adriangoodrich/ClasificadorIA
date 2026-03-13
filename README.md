# 🗂️ Clasificador IA de Archivos

[![Licencia MIT](https://img.shields.io/badge/Licencia-MIT-green.svg)](LICENSE)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![Plataforma](https://img.shields.io/badge/Plataforma-Windows-0078D6?logo=windows)](https://github.com/adriangoodrich/ClasificadorIA/releases)
[![Último commit](https://img.shields.io/github/last-commit/adriangoodrich/ClasificadorIA)](https://github.com/adriangoodrich/ClasificadorIA/commits/main)
[![GitHub release](https://img.shields.io/github/v/release/adriangoodrich/ClasificadorIA?include_prereleases)](https://github.com/adriangoodrich/ClasificadorIA/releases)

> Aplicación de escritorio para Windows que clasifica automáticamente tus archivos en carpetas temáticas utilizando inteligencia artificial (ChatGPT, Claude, DeepSeek, Gemini y más).

---

## 📸 Capturas de pantalla

| Pantalla principal 
|---|---|
| ![Principal](principal.png) | ![Main](main.png) | 

---

## ✨ Características

- 🖥️ **Interfaz gráfica moderna** con WPF, diseño limpio y scroll automático adaptado a ventanas de cualquier tamaño
- 📁 **Selección de carpeta** mediante el diálogo estándar de Windows
- 🎯 **Modos de clasificación** especializados:
  - 🗃️ **Genérico** — clasificación por temática general
  - 🎵 **Música** — por género, artista, época o álbum
  - 🎬 **Películas** — por género, director, año o saga
  - 📺 **Series** — por género, plataforma o temporada
  - 📚 **Libros** — por género literario, autor o colección
- 🔢 **Niveles de profundidad** ajustables: 5, 10 o 15 categorías aproximadas para controlar la granularidad
- 📝 **Generación de prompt inteligente** personalizado según el modo, criterio y profundidad seleccionados
- 📋 **Copiar o guardar prompt** como archivo `.txt` para pegarlo en cualquier IA
- 🤖 **Compatible con cualquier IA** — ChatGPT, Claude, DeepSeek, Gemini, Mistral, etc.
- 📥 **Carga de respuesta flexible** — pega directamente desde el portapapeles o importa un archivo `.txt` / `.json`
- 👁️ **Vista previa de categorías** con número de archivos incluidos y listado de los primeros elementos
- ⚙️ **Procesamiento final** con opción de **copiar** o **mover** archivos a las subcarpetas creadas
- 📊 **Ventana de progreso flotante** con barra de progreso en tiempo real y botón de cancelar
- 🛡️ **Manejo robusto de nombres** — normalización de mayúsculas/minúsculas y caracteres especiales para evitar errores de "archivo no encontrado"

---

## 🖥️ Requisitos del sistema

| Requisito | Versión mínima |
|---|---|
| Sistema operativo | Windows 10 / Windows 11 |
| .NET Runtime | [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) o superior |
| Arquitectura | x64 |
| RAM | 256 MB |

---

## 🚀 Instalación

### Opción 1 — Descarga directa (recomendado)

1. Ve a la sección [**Releases**](https://github.com/adriangoodrich/ClasificadorIA/releases)
2. Descarga el archivo `.zip` de la última versión
3. Extrae el contenido en la carpeta que prefieras
4. Ejecuta `ClasificadorIA.exe`

> **Nota:** Asegúrate de tener instalado el [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) antes de ejecutar la aplicación.

---

### Opción 2 — Compilar desde el código fuente

#### Prerrequisitos

- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/) con extensión C#
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

#### Pasos

```bash
# Clona el repositorio
git clone https://github.com/adriangoodrich/ClasificadorIA.git

# Entra en la carpeta del proyecto
cd ClasificadorIA

# Compila el proyecto
dotnet build --configuration Release

# Ejecuta la aplicación
dotnet run
```

---

## 📖 Guía de uso

### Paso 1 — Selecciona una carpeta

Haz clic en **"Seleccionar carpeta"** y elige el directorio que contiene los archivos que deseas clasificar.

### Paso 2 — Configura la clasificación

Elige el **modo** más adecuado para tus archivos (Genérico, Música, Películas, etc.), selecciona un **criterio** de clasificación y ajusta el **nivel de profundidad** (número de categorías).

### Paso 3 — Genera el prompt

Haz clic en **"Generar prompt"**. La aplicación creará automáticamente un texto optimizado con la lista de tus archivos y las instrucciones para la IA.

### Paso 4 — Copia el prompt y consúltalo en tu IA favorita

Usa el botón **"Copiar al portapapeles"** o guárdalo como `.txt`. Pégalo en ChatGPT, Claude, DeepSeek, Gemini u otra IA de tu elección.

### Paso 5 — Carga la respuesta de la IA

Una vez la IA te devuelva la clasificación en formato JSON, vuelve a la aplicación y:
- Pega la respuesta con **"Pegar desde portapapeles"**, o
- Importa el archivo con **"Cargar desde archivo"**

### Paso 6 — Revisa las categorías

La aplicación mostrará todas las categorías detectadas con el número de archivos y una vista previa de los primeros elementos. Verifica que todo sea correcto.

### Paso 7 — Aplica la clasificación

Elige si deseas **copiar** o **mover** los archivos, y haz clic en **"Procesar"**. Una ventana de progreso te mostrará el avance en tiempo real. Puedes cancelar el proceso en cualquier momento.

---

## 🛠️ Tecnologías utilizadas

| Tecnología | Descripción |
|---|---|
| [C# / .NET 8.0](https://dotnet.microsoft.com/) | Lenguaje y plataforma principal |
| [WPF (Windows Presentation Foundation)](https://learn.microsoft.com/es-es/dotnet/desktop/wpf/) | Framework de interfaz gráfica |
| [System.Text.Json](https://learn.microsoft.com/es-es/dotnet/standard/serialization/system-text-json/overview) | Deserialización de respuestas JSON de la IA |
| [CancellationToken](https://learn.microsoft.com/es-es/dotnet/standard/threading/cancellation-in-managed-threads) | Cancelación de operaciones en progreso |

---

## 🤝 Contribuir

¡Las contribuciones son bienvenidas! Si quieres mejorar este proyecto:

1. Haz un **fork** del repositorio
2. Crea una nueva rama con tu mejora:
   ```bash
   git checkout -b feature/mi-mejora
   ```
3. Realiza tus cambios y haz commit:
   ```bash
   git commit -m "feat: descripción de la mejora"
   ```
4. Sube los cambios a tu fork:
   ```bash
   git push origin feature/mi-mejora
   ```
5. Abre un **Pull Request** explicando los cambios realizados

### 🐛 Reportar errores

Si encuentras algún bug, abre un [Issue](https://github.com/adriangoodrich/ClasificadorIA/issues) con:
- Descripción del problema
- Pasos para reproducirlo
- Versión de la aplicación y de Windows

---

## 📄 Licencia

Este proyecto está distribuido bajo la licencia **MIT**. Consulta el archivo [LICENSE](LICENSE) para más detalles.

```
MIT License — Copyright (c) 2024 adriangoodrich
```

---

<div align="center">
  Hecho por <a href="https://github.com/adriangoodrich">adriangoodrich</a>
  <br><br>
  ⭐ Si te resulta útil, ¡dale una estrella al repositorio!
</div>
