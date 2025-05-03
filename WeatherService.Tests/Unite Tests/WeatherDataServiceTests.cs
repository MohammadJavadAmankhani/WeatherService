using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherService.Application.Services;
using WeatherService.Core.Interfaces;
using WeatherService.Core.Models;

namespace WeatherService.Tests.Unite_Tests
{
    public class WeatherDataServiceTests
    {
        private readonly Mock<IWeatherService> _weatherServiceMock;
        private readonly Mock<IWeatherRepository> _weatherRepositoryMock;
        private readonly WeatherDataService _weatherDataService;

        public WeatherDataServiceTests()
        {
            _weatherServiceMock = new Mock<IWeatherService>();
            _weatherRepositoryMock = new Mock<IWeatherRepository>();
            _weatherDataService = new WeatherDataService(_weatherServiceMock.Object, _weatherRepositoryMock.Object);
        }

        [Fact]
        public async Task GetWeatherDataAsync_FreshDataAvailable_ReturnsFreshData()
        {
            // Arrange
            var expectedWeatherData = new WeatherData
            {
                RetrievedAt = DateTime.UtcNow,
                RawJson = "{\"temperature\": 20.5}"
            };

            _weatherServiceMock.Setup(x => x.GetWeatherDataAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(expectedWeatherData);

            // Act
            var result = await _weatherDataService.GetWeatherDataAsync(52.52, 13.41);

            // Assert
            Assert.Equal(expectedWeatherData.RawJson, result);
            _weatherRepositoryMock.Verify(x => x.SaveWeatherDataAsync(It.IsAny<WeatherData>()), Times.Once);
        }

        [Fact]
        public async Task GetWeatherDataAsync_NoFreshData_ReturnsLatestFromDatabase()
        {
            // Arrange
            var expectedWeatherData = new WeatherData
            {
                RetrievedAt = DateTime.UtcNow.AddHours(-1),
                RawJson = "{\"temperature\": 20.5}"
            };

            _weatherServiceMock.Setup(x => x.GetWeatherDataAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync((WeatherData)null);

            _weatherRepositoryMock.Setup(x => x.GetLatestWeatherDataAsync())
                .ReturnsAsync(expectedWeatherData);

            // Act
            var result = await _weatherDataService.GetWeatherDataAsync(52.52, 13.41);

            // Assert
            Assert.Equal(expectedWeatherData.RawJson, result);
        }

        [Fact]
        public async Task GetWeatherDataAsync_NoDataAvailable_ReturnsNull()
        {
            // Arrange
            _weatherServiceMock.Setup(x => x.GetWeatherDataAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync((WeatherData)null);

            _weatherRepositoryMock.Setup(x => x.GetLatestWeatherDataAsync())
                .ReturnsAsync((WeatherData)null);

            // Act
            var result = await _weatherDataService.GetWeatherDataAsync(52.52, 13.41);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetWeatherDataAsync_ServiceThrowsException_ReturnsLatestFromDatabase()
        {
            // Arrange
            var expectedWeatherData = new WeatherData
            {
                RetrievedAt = DateTime.UtcNow.AddHours(-1),
                RawJson = "{\"temperature\": 20.5}"
            };

            _weatherServiceMock.Setup(x => x.GetWeatherDataAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ThrowsAsync(new Exception("Service error"));

            _weatherRepositoryMock.Setup(x => x.GetLatestWeatherDataAsync())
                .ReturnsAsync(expectedWeatherData);

            // Act
            var result = await _weatherDataService.GetWeatherDataAsync(52.52, 13.41);

            // Assert
            Assert.Equal(expectedWeatherData.RawJson, result);
        }
    }
}
