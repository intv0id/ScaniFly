using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScaniFly.Models;

namespace ScaniFly.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private string _monitoredDirectory = string.Empty;
    public string MonitoredDirectory
    {
        get => _monitoredDirectory;
        set { _monitoredDirectory = value; OnPropertyChanged(); }
    }

    private string _outputDirectory = string.Empty;
    public string OutputDirectory
    {
        get => _outputDirectory;
        set { _outputDirectory = value; OnPropertyChanged(); }
    }

    private string _ollamaModel = string.Empty;
    public string OllamaModel
    {
        get => _ollamaModel;
        set { _ollamaModel = value; OnPropertyChanged(); }
    }

    private string _ocrMethod = "Vision LLM";
    public string OcrMethod
    {
        get => _ocrMethod;
        set { _ocrMethod = value; OnPropertyChanged(); }
    }

    public void LoadFromSettings(AppSettings settings)
    {
        MonitoredDirectory = settings.MonitoredDirectory;
        OutputDirectory = settings.OutputDirectory;
        OllamaModel = settings.OllamaModel;
        OcrMethod = settings.OcrMethod;
    }

    public AppSettings ToSettings()
    {
        return new AppSettings
        {
            MonitoredDirectory = MonitoredDirectory,
            OutputDirectory = OutputDirectory,
            OllamaModel = OllamaModel,
            OcrMethod = OcrMethod
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
