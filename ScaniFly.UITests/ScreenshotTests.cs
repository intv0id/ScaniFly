using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Microsoft.Playwright;

namespace ScaniFly.UITests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ScreenshotTests : PageTest
{
    private const string BaseUrl = "http://localhost:5000";

    [Test]
    public async Task TakeProposalsScreenshot()
    {
        await Page.GotoAsync(BaseUrl);
        // Wait for page to render
        await Page.WaitForTimeoutAsync(2000);

        var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "../../../../docs");
        Directory.CreateDirectory(dir);

        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = Path.Combine(dir, "proposals.png"),
            FullPage = true
        });
    }

    [Test]
    public async Task TakeSettingsScreenshot()
    {
        await Page.GotoAsync($"{BaseUrl}/settings");
        // Wait for page to render
        await Page.WaitForTimeoutAsync(2000);

        var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "../../../../docs");
        Directory.CreateDirectory(dir);

        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = Path.Combine(dir, "settings.png"),
            FullPage = true
        });
    }
}
