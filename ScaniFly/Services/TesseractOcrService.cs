namespace ScaniFly.Services;

public class TesseractOcrService : IOcrService
{
    public string MethodName => "Tesseract";

    public Task<OcrResult> ExtractAsync(string pdfPath)
    {
        return Task.FromResult(new OcrResult
        {
            ExtractedText = "[Mock Tesseract Output] Document extracted using Tesseract fallback. Tax return for 2023."
        });
    }
}
