namespace ScaniFly.Models;

public class AppSettings
{
    public string MonitoredDirectory { get; set; } = string.Empty;
    public string OutputDirectory { get; set; } = string.Empty;
    public string OllamaModel { get; set; } = string.Empty;
    public string OcrMethod { get; set; } = "Vision LLM";
}
