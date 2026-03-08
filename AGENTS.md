# ScaniFly AI Agent Documentation

## Architecture
ScaniFly is built using **.NET 8 Blazor Server** wrapped inside an **Electron.NET** shell to provide a true cross-platform desktop experience without compiling native MAUI code on environments that lack full mobile/desktop SDKs.

- **Frontend**: Blazor Components (`ScaniFly/Components/Pages`) with Bootstrap 5.
- **Backend/Logic**: C# ASP.NET Core services injected via Dependency Injection (`ScaniFly/Services`).
- **Desktop Shell**: Electron.NET handles System Tray, Notifications, and the Auto-Updater logic natively via Node.js bindings.

## OCR Strategies
The application implements an `IOcrService` interface with 3 strategies based on the user's settings selection:
1. `VisionLlmOcrService`: Renders the first 3 pages of a PDF into Base64 images using `PDFiumCore` and `System.Drawing.Common`. These images are sent directly to a Vision model (like `llava`) in Ollama.
2. `TesseractOcrService`: (Mock fallback in this sandbox) Would use Tesseract.NET to extract raw text, then send to a standard LLM.
3. `NativeOsOcrService`: (Mock fallback in this sandbox) Would use `Windows.Media.Ocr` or Apple's `Vision` API.

## Building Locally
To build and run the application locally, ensure you have the .NET 8 SDK, Node.js, and the ElectronNET.CLI global tool installed.

```bash
cd ScaniFly
dotnet tool install ElectronNET.CLI -g
electronize start
```

## AI Instructions for Future Development
- **Modifying UI**: Only edit `.razor` files in the `Components` folder. Do not edit `wwwroot` HTML unless adding global scripts.
- **Platform Specific Code**: Use `HybridSupport.IsElectronActive` checks before invoking any `ElectronNET.API` methods to ensure the app doesn't crash if run as a standard web app for debugging.
- **Mocking for Tests**: Always mock `HttpClient` and `IOcrService` when writing xUnit tests for `LlmSemanticService` to prevent CI pipelines from hanging waiting for Ollama.
