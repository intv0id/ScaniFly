using System.Text.Json;
using ScaniFly.Models;

namespace ScaniFly.Services;

public class LlmSemanticService
{
    private readonly OllamaService _ollamaService;
    private readonly IEnumerable<IOcrService> _ocrServices;
    private readonly SettingsService _settingsService;

    public LlmSemanticService(OllamaService ollamaService, IEnumerable<IOcrService> ocrServices, SettingsService settingsService)
    {
        _ollamaService = ollamaService;
        _ocrServices = ocrServices;
        _settingsService = settingsService;
    }

    public async Task<DocumentProposal> AnalyzeDocumentAsync(string pdfPath)
    {
        var settings = await _settingsService.GetSettingsAsync();

        var ocrService = _ocrServices.FirstOrDefault(s => s.MethodName == settings.OcrMethod)
                         ?? _ocrServices.First();

        var ocrResult = await ocrService.ExtractAsync(pdfPath);

        string prompt = $@"
You are a document organization assistant. Your task is to analyze the following scanned document and provide:
1. A concise, descriptive filename ending in .pdf (e.g. 'Tax_Return_2023.pdf').
2. A suggested subfolder path relative to the root '{settings.OutputDirectory}' (e.g. '\\Taxes\\2023').

Output ONLY a valid JSON object with the keys 'SuggestedTitle' and 'SuggestedDestination'.
";
        if (!string.IsNullOrEmpty(ocrResult.ExtractedText))
        {
            prompt += $"\n\nExtracted Text:\n{ocrResult.ExtractedText}";
        }

        string jsonResponse = "{}";
        try
        {
            jsonResponse = await _ollamaService.GenerateAsync(new OllamaGenerateRequest
            {
                Model = settings.OllamaModel,
                Prompt = prompt,
                Images = ocrResult.Base64Images.Any() ? ocrResult.Base64Images : null,
                Stream = false,
                Format = "json"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LLM Error: {ex.Message}");
            // Fallback
            return new DocumentProposal
            {
                OriginalFilePath = pdfPath,
                SuggestedTitle = $"Scan_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                SuggestedDestinationFolder = "\\Uncategorized"
            };
        }

        LlmExtractionResult? extraction = null;
        try
        {
            extraction = JsonSerializer.Deserialize<LlmExtractionResult>(jsonResponse);
        }
        catch
        {
            // Fallback for bad JSON
        }

        return new DocumentProposal
        {
            OriginalFilePath = pdfPath,
            SuggestedTitle = string.IsNullOrEmpty(extraction?.SuggestedTitle) ? Path.GetFileName(pdfPath) : extraction.SuggestedTitle,
            SuggestedDestinationFolder = string.IsNullOrEmpty(extraction?.SuggestedDestinationFolder) ? "\\Uncategorized" : extraction.SuggestedDestinationFolder
        };
    }
}
