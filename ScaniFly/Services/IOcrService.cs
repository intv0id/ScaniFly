namespace ScaniFly.Services;

public interface IOcrService
{
    string MethodName { get; }
    Task<OcrResult> ExtractAsync(string pdfPath);
}

public class OcrResult
{
    public string ExtractedText { get; set; } = string.Empty;
    public List<string> Base64Images { get; set; } = new();
}
