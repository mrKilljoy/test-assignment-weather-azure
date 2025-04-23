using Assignment.AzureWeather.Application.DTO;
using Assignment.AzureWeather.Application.Interfaces;
using Assignment.AzureWeather.Domain.Models;
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
                .Select(Map)
                .ToList();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetStatistics));
            throw;
        }
    }

    public async Task<List<WeatherStatisticsDto>> GetStatistics(DateTime from, DateTime to)
    {
        try
        {
            var items = await _repository.GetByDatesAsync(from, to);
            var result = items
                .Select(Map)
                .ToList();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(GetStatistics));
            throw;
        }
    }

    private WeatherStatisticsDto Map(WeatherInfo sourceModel)
    {
        return new WeatherStatisticsDto()
        {
            City = sourceModel.City,
            Country = sourceModel.Country,
            Temperature = sourceModel.Temperature,
            DateCreated = sourceModel.DateCreated
        };
    }
}