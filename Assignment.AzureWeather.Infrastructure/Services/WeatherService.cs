using System.Web;
using Assignment.AzureWeather.Application.DTO;
using Assignment.AzureWeather.Application.Interfaces;
using Assignment.AzureWeather.Infrastructure.Configuration;
using Assignment.AzureWeather.Infrastructure.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Assignment.AzureWeather.Infrastructure.Services;

public class WeatherService : IWeatherService
{
    private readonly ILogger<WeatherService> _logger;
    private readonly WeatherServiceConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherService(
        ILogger<WeatherService> logger,
        IOptions<WeatherServiceConfiguration> options,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _config = options?.Value;
    }
    
    public async Task<WeatherDataDto> GetCurrentWeatherAsync(string location)
    {
        try
        {
            if (string.IsNullOrEmpty(location))
                throw new ArgumentNullException(nameof(location));

            var url = BuildUrl(location);

            using var client = CreateClient();
            using var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var rawContent = await response.Content.ReadAsStringAsync();
                
                var responseModel = JsonConvert.DeserializeObject<WeatherResponseDto>(rawContent);
                
                _logger.LogTrace($"Weather data for '{location}' received successfully.");

                return new WeatherDataDto()
                {
                    City = responseModel?.Name,
                    Country = responseModel?.Sys?.Country,
                    Temperature = responseModel?.Main?.Temp ?? default,
                    RawData = rawContent
                };
            }

            _logger.LogTrace($"Failed to acquire weather data for '{location}'.");

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetCurrentWeatherAsync));
            throw;
        }
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient();

    private string BuildUrl(string location)
    {
        string apiKey = GetKey();
        string units = GetUnits();
        string baseUrl = GetBaseUrl();
            
        string url = $"{baseUrl}/data/2.5/weather";
        var uriBuilder = new UriBuilder(url);

        var query = HttpUtility.ParseQueryString(url);
        query["q"] = location;
        query["appid"] = apiKey;
        if (!string.IsNullOrEmpty(units))
        {
            query["units"] = units;
        }

        uriBuilder.Query = query.ToString();

        return uriBuilder.ToString();
    }

    private string GetUnits() => _config.Units;
    
    private string GetKey() => _config.ApiKey;

    private string GetBaseUrl() => _config.ServiceUrl;
}