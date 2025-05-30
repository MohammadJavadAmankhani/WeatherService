using Moq;
using Moq.Protected;
using Polly;
using System.Net;
using WeatherService.Infrastructure.Resilience;
using WeatherService.Infrastructure.Services;

namespace WeatherService.Tests.Unite_Tests
{
    public class OpenMeteoWeatherServiceTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly OpenMeteoWeatherService _weatherService;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<IPolicyFactory> _policyFactoryMock;

        public OpenMeteoWeatherServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            _policyFactoryMock = new Mock<IPolicyFactory>();
            var noOpPolicy = Policy.NoOpAsync<HttpResponseMessage>();
            _policyFactoryMock.Setup(x => x.CreateHttpPolicy())
                .Returns(noOpPolicy);

            _weatherService = new OpenMeteoWeatherService(_httpClientFactoryMock.Object, _policyFactoryMock.Object);
        }

        [Fact]
        public async Task GetWeatherDataAsync_ValidCoordinates_ReturnsWeatherData()
        {
            // Arrange
            var expectedJson = "{\"temperature\": 20.5}";
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedJson)
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act
            var result = await _weatherService.GetWeatherDataAsync(52.52, 13.41);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedJson, result.RawJson);
            Assert.True((DateTime.UtcNow - result.RetrievedAt).TotalSeconds < 5);
        }

        [Fact]
        public async Task GetWeatherDataAsync_InvalidResponse_ReturnsNull()
        {
            // Arrange
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act
            var result = await _weatherService.GetWeatherDataAsync(52.52, 13.41);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetWeatherDataAsync_RequestTimeout_ReturnsNull()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new TaskCanceledException());

            // Act
            var result = await _weatherService.GetWeatherDataAsync(52.52, 13.41);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetWeatherDataAsync_NetworkError_ReturnsNull()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException());

            // Act
            var result = await _weatherService.GetWeatherDataAsync(52.52, 13.41);

            // Assert
            Assert.Null(result);
        }
    }
}
