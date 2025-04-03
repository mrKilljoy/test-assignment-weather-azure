using Assignment.AzureWeather.Application.Interfaces;
using Assignment.AzureWeather.Infrastructure.Exceptions;
using Assignment.AzureWeather.Infrastructure.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Assignment.AzureWeather.Function.Functions;

public class CheckWeatherFunction
{
    private readonly ILogger<CheckWeatherFunction> _logger;
    private readonly IConfiguration _configuration;
    private readonly IWeatherService _weatherService;
    private readonly IWeatherLogService _weatherLogService;

    public CheckWeatherFunction(
        ILogger<CheckWeatherFunction> logger,
        IConfiguration configuration,
        IWeatherService weatherService,
        IWeatherLogService weatherLogService)
    {
        _logger = logger;
        _configuration = configuration;
        _weatherService = weatherService;
        _weatherLogService = weatherLogService;
    }

    [Function("CheckWeather")]
    public async Task RunAsync([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
    {
        try
        {
            _logger.LogInformation("Fetching weather data...");
            
            var data = await _weatherService.GetCurrentWeatherAsync(GetCityName());
            await _weatherLogService.LogAsync(data is not null, data?.RawData);
            
            _logger.LogInformation("Weather data acquired.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(CheckWeatherFunction));
        }
    }

    private string GetCityName() =>
        _configuration["WeatherDataCityName"] ??
        throw new ConfigurationItemNotFoundException("WeatherDataCityName");
}