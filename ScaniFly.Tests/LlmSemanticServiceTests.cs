using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ScaniFly.Services;
using ScaniFly.Models;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace ScaniFly.Tests;

public class LlmSemanticServiceTests
{
    [Fact]
    public async Task AnalyzeDocumentAsync_ReturnsProperProposal_WhenLLMReturnsJson()
    {
        // Arrange
        var expectedJson = "{ \"SuggestedTitle\": \"Invoice_2023.pdf\", \"SuggestedDestination\": \"\\\\Invoices\\\\2023\" }";

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new OllamaGenerateResponse { Response = expectedJson }))
            });

        var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost:11434") };
        var ollamaService = new OllamaService(httpClient);

        var ocrMock = new Mock<IOcrService>();
        ocrMock.Setup(x => x.MethodName).Returns("Vision LLM");
        ocrMock.Setup(x => x.ExtractAsync(It.IsAny<string>())).ReturnsAsync(new OcrResult { ExtractedText = "Invoice data" });

        var settingsMock = new Mock<SettingsService>();
        settingsMock.Setup(x => x.GetSettingsAsync()).ReturnsAsync(new AppSettings { OcrMethod = "Vision LLM", OutputDirectory = "C:\\Out" });

        var service = new LlmSemanticService(ollamaService, new[] { ocrMock.Object }, settingsMock.Object);

        // Act
        var result = await service.AnalyzeDocumentAsync("test.pdf");

        // Assert
        Assert.Equal("Invoice_2023.pdf", result.SuggestedTitle);
        Assert.Equal("\\Invoices\\2023", result.SuggestedDestinationFolder);
        Assert.Equal("test.pdf", result.OriginalFilePath);
    }
}
