using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ScaniFly.Services;
using Xunit;

namespace ScaniFly.Tests;

public class OllamaServiceTests
{
    [Fact]
    public async Task GetAvailableModelsAsync_ReturnsModelsList_WhenApiSucceeds()
    {
        // Arrange
        var expectedModels = new List<string> { "llama3", "mistral" };
        var tagsResponse = new OllamaTagsResponse
        {
            Models = expectedModels.Select(m => new OllamaModel { Name = m }).ToList()
        };
        var jsonResponse = JsonSerializer.Serialize(tagsResponse);

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
                Content = new StringContent(jsonResponse)
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new OllamaService(httpClient);

        // Act
        var result = await service.GetAvailableModelsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains("llama3", result);
        Assert.Contains("mistral", result);
    }
}
