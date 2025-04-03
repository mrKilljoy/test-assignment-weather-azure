using System.Net;
using Assignment.AzureWeather.Function.Functions;
using Assignment.AzureWeather.Infrastructure.Interfaces;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Assignment.AzureWeather.Tests.Functions;

[TestFixture]
public class GetPayloadFunctionTests
{
    private Mock<IWeatherLogService> _weatherLogServiceMock;
    private Mock<ILogger<GetPayloadFunction>> _loggerMock;
    private GetPayloadFunction _function;

    [SetUp]
    public void Setup()
    {
        _weatherLogServiceMock = new Mock<IWeatherLogService>();
        _loggerMock = new Mock<ILogger<GetPayloadFunction>>();

        _function = new GetPayloadFunction(_loggerMock.Object, _weatherLogServiceMock.Object);
    }

    [Test]
    public async Task RunAsync_ValidDateTime_ReturnsOkResponse()
    {
        // Arrange
        var requestMock = BuildHttpRequestData();
        var date = "20230101";
        var time = "120000";
        string key = $"weather-{date}-{time}.json";
        var expectedPayload = "Sample payload content";
        _weatherLogServiceMock
            .Setup(service => service.GetLogPayloadAsync(key))
            .ReturnsAsync(expectedPayload);

        // Act
        var result = await _function.RunAsync(requestMock, date, time);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        _weatherLogServiceMock.Verify(service => service.GetLogPayloadAsync(key), Times.Once);
    }

    [Test]
    public async Task RunAsync_MissingDateTime_ReturnsBadRequest()
    {
        // Arrange
        var requestMock = BuildHttpRequestData();
        string date = null;
        string time = null;

        // Act
        var result = await _function.RunAsync(requestMock, date, time);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task RunAsync_PayloadNotFound_ReturnsNotFound()
    {
        // Arrange
        var requestMock = BuildHttpRequestData();
        var date = "20230101";
        var time = "120000";
        string key = $"weather-{date}-{time}.json";
        _weatherLogServiceMock
            .Setup(service => service.GetLogPayloadAsync(key))
            .ReturnsAsync((string)null);

        // Act
        var result = await _function.RunAsync(requestMock, date, time);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        _weatherLogServiceMock.Verify(service => service.GetLogPayloadAsync(key), Times.Once);
    }

    [Test]
    public async Task RunAsync_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var requestMock = BuildHttpRequestData();
        var date = "2023-01-01";
        var time = "120000";
        string key = $"weather-{date}-{time}.json";
        _weatherLogServiceMock
            .Setup(service => service.GetLogPayloadAsync(key))
            .ThrowsAsync(new Exception("Service error"));
        
        // Act
        var result = await _function.RunAsync(requestMock, date, time);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    private HttpRequestData BuildHttpRequestData()
    {
        var requestDataBuilder = new WorkerHttpFake.HttpRequestDataBuilder();
        return requestDataBuilder.Build();
    }
}