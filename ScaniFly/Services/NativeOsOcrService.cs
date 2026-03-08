namespace ScaniFly.Services;

public class NativeOsOcrService : IOcrService
{
    public string MethodName => "Native OS";

    public Task<OcrResult> ExtractAsync(string pdfPath)
    {
        return Task.FromResult(new OcrResult
        {
            ExtractedText = "[Mock Native OS Output] Document extracted natively. Invoice for internet services."
        });
    }
}
