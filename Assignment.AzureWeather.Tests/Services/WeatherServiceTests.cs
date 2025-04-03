using System.Net;
using Assignment.AzureWeather.Infrastructure.Configuration;
using Assignment.AzureWeather.Infrastructure.DTO;
using Assignment.AzureWeather.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace Assignment.AzureWeather.Tests.Services;

[TestFixture]
public class WeatherServiceTests
{
    private Mock<ILogger<WeatherService>> _loggerMock;
    private Mock<IOptions<WeatherServiceConfiguration>> _optionsMock;
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private WeatherServiceConfiguration _config;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<WeatherService>>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        _config = new WeatherServiceConfiguration
        {
            ApiKey = "test_api_key",
            Units = "metric",
            ServiceUrl = "http://api.openweathermap.org"
        };

        _optionsMock = new Mock<IOptions<WeatherServiceConfiguration>>();
        _optionsMock.Setup(x => x.Value).Returns(_config);
    }

    // Helper method to create an HttpClient using a fake message handler.
    private HttpClient CreateHttpClient(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            // Setup the SendAsync method behavior
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        return new HttpClient(handlerMock.Object);
    }

    [Test]
    public void GetCurrentWeatherAsync_NullOrEmptyLocation_ThrowsArgumentNullException()
    {
        // Arrange
        var weatherService = new WeatherService(_loggerMock.Object, _optionsMock.Object, _httpClientFactoryMock.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await weatherService.GetCurrentWeatherAsync(null));
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await weatherService.GetCurrentWeatherAsync(string.Empty));
    }

    [Test]
    public async Task GetCurrentWeatherAsync_SuccessfulResponse_ReturnsWeatherDataDto()
    {
        // Arrange: Create a fake successful HTTP response with JSON content
        var weatherResponse = new WeatherResponseDto
        {
            Name = "Test City",
            Sys = new Sys() { Country = "TC" },
            Main = new Main() { Temp = 25.5m }
        };

        string jsonResponse = JsonConvert.SerializeObject(weatherResponse);
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        // Setup the IHttpClientFactory mock to return the HttpClient with our fake response.
        var httpClient = CreateHttpClient(responseMessage);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var weatherService = new WeatherService(_loggerMock.Object, _optionsMock.Object, _httpClientFactoryMock.Object);

        // Act
        var result = await weatherService.GetCurrentWeatherAsync("Test City");

        // Assert: Validate that the returned data matches the simulated response.
        Assert.IsNotNull(result);
        Assert.AreEqual("Test City", result.City);
        Assert.AreEqual("TC", result.Country);
        Assert.AreEqual(25.5, result.Temperature);
        Assert.AreEqual(jsonResponse, result.RawData);
    }

    [Test]
    public async Task GetCurrentWeatherAsync_UnsuccessfulResponse_ReturnsNull()
    {
        // Arrange: Create a fake HTTP response with an error status code.
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var httpClient = CreateHttpClient(responseMessage);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var weatherService = new WeatherService(_loggerMock.Object, _optionsMock.Object, _httpClientFactoryMock.Object);

        // Act
        var result = await weatherService.GetCurrentWeatherAsync("Test City");

        // Assert: When the response is unsuccessful, the method returns null.
        Assert.IsNull(result);
    }

    [Test]
    public void GetCurrentWeatherAsync_HttpClientThrowsException_ThrowsException()
    {
        // Arrange: Setup the HttpClient to throw an exception when making the HTTP call.
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Test Exception"));

        var httpClient = new HttpClient(handlerMock.Object);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var weatherService = new WeatherService(_loggerMock.Object, _optionsMock.Object, _httpClientFactoryMock.Object);

        // Act & Assert: Ensure that an exception is thrown and not swallowed.
        Assert.ThrowsAsync<HttpRequestException>(async () => await weatherService.GetCurrentWeatherAsync("Test City"));
    }
}