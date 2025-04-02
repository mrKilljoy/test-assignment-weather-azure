using Assignment.AzureWeather.Application.DTO;
using Assignment.AzureWeather.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Assignment.AzureWeather.Infrastructure.Services;

public class WeatherStatisticsService : IWeatherStatisticsService
{
    private readonly ILogger<WeatherStatisticsService> _logger;
    private readonly IWeatherInfoRepository _repository;

    public WeatherStatisticsService(ILogger<WeatherStatisticsService> logger, IWeatherInfoRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }
    
    public async Task<List<WeatherStatisticsDto>> GetStatistics()
    {
        try
        {
            var items = await _repository.GetAllAsync();
            var result = items
                .Select(x => new WeatherStatisticsDto() { City = x.City, Country = x.Country, Temperature = x.Temperature })
                .ToList(); // todo: map

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetStatistics));
            throw;
        }
    }
}