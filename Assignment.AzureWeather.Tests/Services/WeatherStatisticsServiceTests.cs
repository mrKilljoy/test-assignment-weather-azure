using Assignment.AzureWeather.Application.Interfaces;
using Assignment.AzureWeather.Domain.Models;
using Assignment.AzureWeather.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Assignment.AzureWeather.Tests.Services;

[TestFixture]
public class WeatherStatisticsServiceTests
{
    private Mock<ILogger<WeatherStatisticsService>> _loggerMock;
    private Mock<IWeatherInfoRepository> _repositoryMock;
    private WeatherStatisticsService _weatherStatisticsService;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<WeatherStatisticsService>>();
        _repositoryMock = new Mock<IWeatherInfoRepository>();

        _weatherStatisticsService = new WeatherStatisticsService(
            _loggerMock.Object,
            _repositoryMock.Object
        );
    }

    [Test]
    public async Task GetStatistics_ReturnsMappedStatistics()
    {
        // Arrange
        var weatherInfoList = new List<WeatherInfo>
        {
            new WeatherInfo
            {
                City = "City1",
                Country = "Country1",
                Temperature = 25.5m,
                DateCreated = DateTime.UtcNow
            },
            new WeatherInfo
            {
                City = "City2",
                Country = "Country2",
                Temperature = 30.0m,
                DateCreated = DateTime.UtcNow.AddHours(-1)
            }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(weatherInfoList);

        // Act
        var result = await _weatherStatisticsService.GetStatistics();

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].City, Is.EqualTo("City1"));
        Assert.That(result[0].Country, Is.EqualTo("Country1"));
        Assert.That(result[0].Temperature, Is.EqualTo(25.5));
        Assert.That(result[0].DateCreated, Is.EqualTo(weatherInfoList[0].DateCreated));
        Assert.That(result[1].City, Is.EqualTo("City2"));
        Assert.That(result[1].Country, Is.EqualTo("Country2"));
        Assert.That(result[1].Temperature, Is.EqualTo(30.0));
        Assert.That(result[1].DateCreated, Is.EqualTo(weatherInfoList[1].DateCreated));
    }

    [Test]
    public void GetStatistics_ThrowsException_LogsError()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await _weatherStatisticsService.GetStatistics());
        Assert.That(ex.Message, Is.EqualTo("Database error"));
    }
}