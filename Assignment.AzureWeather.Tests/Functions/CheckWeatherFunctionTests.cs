using Assignment.AzureWeather.Application.DTO;
using Assignment.AzureWeather.Application.Interfaces;
using Assignment.AzureWeather.Function.Functions;
using Assignment.AzureWeather.Infrastructure.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Assignment.AzureWeather.Tests.Functions;

[TestFixture]
    public class CheckWeatherFunctionTests
    {
        private Mock<ILogger<CheckWeatherFunction>> _loggerMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IWeatherService> _weatherServiceMock;
        private Mock<IWeatherLogService> _weatherLogServiceMock;
        private CheckWeatherFunction _function;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<CheckWeatherFunction>>();
            _configurationMock = new Mock<IConfiguration>();
            _weatherServiceMock = new Mock<IWeatherService>();
            _weatherLogServiceMock = new Mock<IWeatherLogService>();

            _function = new CheckWeatherFunction(
                _loggerMock.Object,
                _configurationMock.Object,
                _weatherServiceMock.Object,
                _weatherLogServiceMock.Object
            );
        }

        [Test]
        public async Task RunAsync_WeatherDataFetched_LogsSuccess()
        {
            // Arrange
            var cityName = "New York";
            var weatherData = new WeatherDataDto
            {
                City = cityName,
                Country = "US",
                Temperature = 23.5m,
                RawData = "{\"main\": {\"temp\": 23.5}}"
            };
            _configurationMock
                .Setup(c => c["WeatherDataCityName"])
                .Returns(cityName);
            _weatherServiceMock
                .Setup(service => service.GetCurrentWeatherAsync(cityName))
                .ReturnsAsync(weatherData);

            // Act
            await _function.RunAsync(new TimerInfo());

            // Assert
            _weatherLogServiceMock.Verify(service => service.LogAsync(true, weatherData.RawData), Times.Once);
        }

        [Test]
        public async Task RunAsync_NoWeatherData_LogsFailure()
        {
            // Arrange
            var cityName = "New York";
            _configurationMock
                .Setup(c => c["WeatherDataCityName"])
                .Returns(cityName);
            _weatherServiceMock
                .Setup(service => service.GetCurrentWeatherAsync(cityName))
                .ReturnsAsync((WeatherDataDto)null);

            // Act
            await _function.RunAsync(new TimerInfo());

            // Assert
            _weatherLogServiceMock.Verify(service => service.LogAsync(false, null), Times.Once);
        }
    }