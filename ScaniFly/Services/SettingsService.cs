using System.Text.Json;
using ScaniFly.Models;

namespace ScaniFly.Services;

public class SettingsService
{
    private readonly string _settingsFilePath;

    public SettingsService()
    {
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
        if (!File.Exists(_settingsFilePath))
        {
            return new AppSettings();
        }

        try
        {
            string json = await File.ReadAllTextAsync(_settingsFilePath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public virtual async Task SaveSettingsAsync(AppSettings settings)
    {
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_settingsFilePath, json);
    }
}
