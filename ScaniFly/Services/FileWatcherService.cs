using Microsoft.Extensions.Hosting;
using System.IO;

namespace ScaniFly.Services;

public class FileWatcherService : BackgroundService
{
    private readonly SettingsService _settingsService;
    private readonly ProposalStateService _proposalStateService;
    private readonly LlmSemanticService _llmSemanticService;
    private readonly TrayService _trayService;
    private FileSystemWatcher? _watcher;

    public FileWatcherService(SettingsService settingsService, ProposalStateService proposalStateService, LlmSemanticService llmSemanticService, TrayService trayService)
    {
        _settingsService = settingsService;
        _proposalStateService = proposalStateService;
        _llmSemanticService = llmSemanticService;
        _trayService = trayService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = await _settingsService.GetSettingsAsync();

        if (!string.IsNullOrEmpty(settings.MonitoredDirectory) && Directory.Exists(settings.MonitoredDirectory))
        {
            StartWatching(settings.MonitoredDirectory);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);

            var currentSettings = await _settingsService.GetSettingsAsync();
            if (_watcher?.Path != currentSettings.MonitoredDirectory)
            {
                StartWatching(currentSettings.MonitoredDirectory);
            }
        }
    }

    private void StartWatching(string path)
    {
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;

        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
        }

        _watcher = new FileSystemWatcher(path, "*.pdf")
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };

        _watcher.Created += async (s, e) =>
        {
            Console.WriteLine($"New PDF detected: {e.FullPath}");

            // Allow file to finish writing before parsing
            await Task.Delay(1000);

            var proposal = await _llmSemanticService.AnalyzeDocumentAsync(e.FullPath);
            _proposalStateService.AddProposal(proposal);
            _trayService.ShowNotification("New Scan Processed", $"Proposal ready for {proposal.SuggestedTitle}");
        };
    }

    public override void Dispose()
    {
        _watcher?.Dispose();
        base.Dispose();
    }
}
