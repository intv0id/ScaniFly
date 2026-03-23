using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ScaniFly.Services;

public class TrayService
{
    private bool _isTrayInitialized = false;

    public void InitializeTray()
    {
        if (!HybridSupport.IsElectronActive || _isTrayInitialized) return;

        var menuItems = new[]
        {
            new MenuItem {
                Label = "Open ScaniFly",
                Click = async () => {
                    var window = Electron.WindowManager.BrowserWindows.FirstOrDefault();
                    if (window != null) window.Show();
                }
            },
            new MenuItem { Type = MenuType.separator },
            new MenuItem {
                Label = "Quit",
                Click = () => Electron.App.Quit()
            }
        };

        Electron.Tray.Show(Path.Combine(Environment.CurrentDirectory, "wwwroot", "favicon.png"), menuItems);
        Electron.Tray.SetToolTip("ScaniFly - Monitoring Scans");

        _isTrayInitialized = true;
    }

    public void ShowNotification(string title, string body)
    {
        if (!HybridSupport.IsElectronActive) return;

        var notificationOptions = new NotificationOptions(title, body)
        {
            Icon = Path.Combine(Environment.CurrentDirectory, "wwwroot", "favicon.png")
        };

        Electron.Notification.Show(notificationOptions);
    }
}
