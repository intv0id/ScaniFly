using ElectronNET.API;
using ScaniFly.Services;
using ScaniFly.Components;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);

// Add services to the container.
builder.Services.AddSingleton<SettingsService>();
builder.Services.AddSingleton<HistoryService>();
builder.Services.AddSingleton<ProposalStateService>();
builder.Services.AddHttpClient<OllamaService>();
builder.Services.AddSingleton<IOcrService, VisionLlmOcrService>();
builder.Services.AddSingleton<IOcrService, TesseractOcrService>();
builder.Services.AddSingleton<IOcrService, NativeOsOcrService>();
builder.Services.AddSingleton<LlmSemanticService>();
builder.Services.AddSingleton<TrayService>();
builder.Services.AddHttpClient<UpdateService>();
builder.Services.AddSingleton<FileWatcherService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<FileWatcherService>());
builder.Services.AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();
app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<ScaniFly.Components.App>()
    .AddInteractiveServerRenderMode();

if (ElectronNET.API.HybridSupport.IsElectronActive)
{
    Task.Run(async () =>
    {
        var window = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync(new ElectronNET.API.Entities.BrowserWindowOptions
        {
            Width = 1200,
            Height = 800,
            Show = false
        });
        await window.WebContents.Session.ClearCacheAsync();
        window.OnReadyToShow += () => window.Show();
        window.SetTitle("ScaniFly");
    });
}
app.Run();
