using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScaniFly.Services;

public class OllamaService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:11434";

    public OllamaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<bool> IsRunningAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<string>> GetAvailableModelsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<OllamaTagsResponse>("/api/tags");
            if (response?.Models != null)
            {
                return response.Models.Select(m => m.Name).ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching Ollama models: {ex.Message}");
        }
        return new List<string>();
    }

    public async Task<string> GenerateAsync(OllamaGenerateRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/generate", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>();
        return result?.Response ?? "{}";
    }
}

public class OllamaTagsResponse
{
    [JsonPropertyName("models")]
    public List<OllamaModel> Models { get; set; } = new();
}

public class OllamaModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class OllamaGenerateRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;

    [JsonPropertyName("images")]
    public List<string>? Images { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; } = "json";
}

public class OllamaGenerateResponse
{
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;
}

public class LlmExtractionResult
{
    [JsonPropertyName("SuggestedTitle")]
    public string SuggestedTitle { get; set; } = string.Empty;

    [JsonPropertyName("SuggestedDestination")]
    public string SuggestedDestinationFolder { get; set; } = string.Empty;
}
