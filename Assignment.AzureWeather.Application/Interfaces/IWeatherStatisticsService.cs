using Assignment.AzureWeather.Application.DTO;

namespace Assignment.AzureWeather.Application.Interfaces;

public interface IWeatherStatisticsService
{
    Task<List<WeatherStatisticsDto>> GetStatistics();
    
    Task<List<WeatherStatisticsDto>> GetStatistics(DateTime from, DateTime to);
}