using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ElectronNET.API;

namespace ScaniFly.Services;

public class UpdateService
{
    private readonly HttpClient _httpClient;
    private const string RepoApiUrl = "https://api.github.com/repos/YOUR_ORG/ScaniFly/releases/latest";

    public UpdateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ScaniFly-AutoUpdater");
    }

    public async Task<bool> CheckForUpdatesAsync(string currentVersion)
    {
        try
        {
            var release = await _httpClient.GetFromJsonAsync<GithubRelease>(RepoApiUrl);
            if (release != null && release.TagName != $"v{currentVersion}")
            {
                if (HybridSupport.IsElectronActive)
                {
                    var options = new ElectronNET.API.Entities.MessageBoxOptions("Update Available")
                    {
                        Type = ElectronNET.API.Entities.MessageBoxType.info,
                        Title = "New Version Available",
                        Message = $"Version {release.TagName} is available! Would you like to download it?",
                        Buttons = new[] { "Yes", "No" }
                    };

                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    var result = await Electron.Dialog.ShowMessageBoxAsync(mainWindow, options);

                    if (result.Response == 0) // "Yes" clicked
                    {
                        // Open default browser to download link
                        await Electron.Shell.OpenExternalAsync(release.HtmlUrl);
                    }
                }
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update check failed: {ex.Message}");
        }
        return false;
    }
}

public class GithubRelease
{
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;
}
