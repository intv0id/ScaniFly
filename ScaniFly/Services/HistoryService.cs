using System.Text.Json;
using ScaniFly.Models;

namespace ScaniFly.Services;

public class HistoryService
{
    private readonly string _historyFilePath;
    private List<DocumentProposal> _history = new();

    public HistoryService()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appFolder = Path.Combine(appDataPath, "ScaniFly");
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }
        _historyFilePath = Path.Combine(appFolder, "history.json");
    }

    public async Task<List<DocumentProposal>> GetHistoryAsync()
    {
        if (!File.Exists(_historyFilePath))
        {
            return new List<DocumentProposal>();
        }

        try
        {
            string json = await File.ReadAllTextAsync(_historyFilePath);
            _history = JsonSerializer.Deserialize<List<DocumentProposal>>(json) ?? new List<DocumentProposal>();
            return _history;
        }
        catch
        {
            return new List<DocumentProposal>();
        }
    }

    public async Task AddToHistoryAsync(DocumentProposal proposal)
    {
        proposal.ProcessedAt = DateTime.Now;
        proposal.IsAccepted = true;
        _history.Add(proposal);
        await SaveHistoryAsync();
    }

    private async Task SaveHistoryAsync()
    {
        string json = JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_historyFilePath, json);
    }
}
