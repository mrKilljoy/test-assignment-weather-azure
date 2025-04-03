using System.Collections.Specialized;
using System.Net;
using Assignment.AzureWeather.Application.DTO;
using Assignment.AzureWeather.Function.Functions;
using Assignment.AzureWeather.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Assignment.AzureWeather.Tests.Functions;

[TestFixture]
public class QueryLogsFunctionTests
{
    private Mock<IWeatherLogService> _weatherLogServiceMock;
    private Mock<ILoggerFactory> _loggerFactoryMock;
    private Mock<ILogger> _loggerMock;
    private QueryLogsFunction _function;

    [SetUp]
    public void Setup()
    {
        _weatherLogServiceMock = new Mock<IWeatherLogService>();
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _loggerMock = new Mock<ILogger>();

        _loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);

        _function = new QueryLogsFunction(_loggerFactoryMock.Object, _weatherLogServiceMock.Object);
    }

    [Test]
    public async Task RunAsync_ValidDateRange_ReturnsOkResponse()
    {
        // Arrange
        var requestDataBuilder = new WorkerHttpFake.HttpRequestDataBuilder();
        var query = new Dictionary<string, string> { { "from", "2023-01-01" }, { "to", "2023-01-02" } };
        var requestMock = requestDataBuilder.WithQueryParams(ToCollection(query)).Build();
        var expectedResult = new List<WeatherLogDto>
        {
            new WeatherLogDto { Status = "Success", Created = DateTime.UtcNow.AddDays(-1) },
            new WeatherLogDto { Status = "Failure", Created = DateTime.UtcNow }
        };
        _weatherLogServiceMock
            .Setup(service => service.QueryAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _function.RunAsync(requestMock);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task RunAsync_MissingFromQuery_ReturnsBadRequest()
    {
        // Arrange
        var requestDataBuilder = new WorkerHttpFake.HttpRequestDataBuilder();
        var query = new Dictionary<string, string> { { "to", "2023-01-02" } };
        var requestMock = requestDataBuilder.WithQueryParams(ToCollection(query)).Build();

        // Act
        var result = await _function.RunAsync(requestMock);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task RunAsync_InvalidDateFormat_ReturnsBadRequest()
    {
        // Arrange
        var requestDataBuilder = new WorkerHttpFake.HttpRequestDataBuilder();
        var query = new Dictionary<string, string> { { "from", "InvalidDate" }, { "to", "2023-01-02" } };
        var requestMock = requestDataBuilder.WithQueryParams(ToCollection(query)).Build();

        // Act
        var result = await _function.RunAsync(requestMock);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task RunAsync_InvalidDateRange_ReturnsBadRequest()
    {
        // Arrange
        var requestDataBuilder = new WorkerHttpFake.HttpRequestDataBuilder();
        var query = new Dictionary<string, string> { { "from", "2023-01-03" }, { "to", "2023-01-02" } };
        var requestMock = requestDataBuilder.WithQueryParams(ToCollection(query)).Build();

        // Act
        var result = await _function.RunAsync(requestMock);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task RunAsync_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var requestDataBuilder = new WorkerHttpFake.HttpRequestDataBuilder();
        var query = new Dictionary<string, string> { { "from", "2023-01-01" }, { "to", "2023-01-02" } };
        var requestMock = requestDataBuilder.WithQueryParams(ToCollection(query)).Build();
        _weatherLogServiceMock
            .Setup(service => service.QueryAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _function.RunAsync(requestMock);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    private NameValueCollection ToCollection(Dictionary<string, string> dictionary)
    {
        return dictionary.Aggregate(new NameValueCollection(),
            (seed, current) =>
            {
                seed.Add(current.Key, current.Value);
                return seed;
            });
    }
}