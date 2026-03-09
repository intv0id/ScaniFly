using System.Text.Json;
using ScaniFly.Models;

namespace ScaniFly.Services;

public class SettingsService
{
    private readonly string _settingsFilePath;
    private AppSettings? _cachedSettings;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public event Action? OnSettingsChanged;

    public SettingsService()
    {
        // Environment.SpecialFolder.ApplicationData resolves to %APPDATA% on Windows and ~/.config on Mac/Linux
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appFolder = Path.Combine(appDataPath, "ScaniFly");

        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }

        _settingsFilePath = Path.Combine(appFolder, "settings.json");
    }

    public virtual async Task<AppSettings> GetSettingsAsync()
    {
        if (_cachedSettings != null)
        {
            return _cachedSettings;
        }

        await _semaphore.WaitAsync();
        try
        {
            // Double-check locking pattern
            if (_cachedSettings != null)
            {
                return _cachedSettings;
            }

            if (!File.Exists(_settingsFilePath))
            {
                _cachedSettings = new AppSettings();
                return _cachedSettings;
            }

            try
            {
                string json = await File.ReadAllTextAsync(_settingsFilePath);
                _cachedSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch
            {
                _cachedSettings = new AppSettings();
            }

            return _cachedSettings;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public virtual async Task SaveSettingsAsync(AppSettings settings)
    {
        await _semaphore.WaitAsync();
        try
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_settingsFilePath, json);

            // Sync in-memory cache
            _cachedSettings = settings;
        }
        finally
        {
            _semaphore.Release();
        }

        OnSettingsChanged?.Invoke();
    }

    public async Task<bool> IsConfiguredAsync()
    {
        var settings = await GetSettingsAsync();
        return !string.IsNullOrEmpty(settings.MonitoredDirectory)
            && !string.IsNullOrEmpty(settings.OutputDirectory);
    }
}
