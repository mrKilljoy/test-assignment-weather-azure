using Assignment.AzureWeather.Application.Interfaces;
using Assignment.AzureWeather.Domain.Models;
using Assignment.AzureWeather.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Assignment.AzureWeather.Api.Services;

public class WeatherBackgroundService : BackgroundService
{
    private readonly ILogger<WeatherBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<WeatherWorkerConfiguration> _options;

    public WeatherBackgroundService(
        ILogger<WeatherBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptions<WeatherWorkerConfiguration> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting execution...");

            var locations = GetLocations();
            var intervalSeconds = GetInterval();
            
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(intervalSeconds));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = _serviceProvider.CreateScope();
                var weatherService = scope.ServiceProvider.GetRequiredService<IWeatherService>();
                var repo = scope.ServiceProvider.GetRequiredService<IWeatherInfoRepository>();

                foreach (var location in locations)
                {
                    var data = await weatherService.GetCurrentWeatherAsync(location);
                    if (data is null)
                    {
                        _logger.LogInformation($"Location not found ('{location}')");
                        continue;
                    }

                    var item = new WeatherInfo()
                    {
                        Id = Guid.NewGuid(), City = data.City, Country = data.Country, Temperature = data.Temperature,
                        DateCreated = DateTime.UtcNow
                    };
                    await repo.AddAsync(item);
                    
                    _logger.LogInformation($"Weather data saved ('{location}')");
                }
            }
            
            _logger.LogInformation("Execution completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has occurred.");
        }
    }

    private List<string> GetLocations() => _options.Value?.Locations ?? new List<string>();

    private int GetInterval() => _options.Value?.IntervalSeconds ?? default;
}