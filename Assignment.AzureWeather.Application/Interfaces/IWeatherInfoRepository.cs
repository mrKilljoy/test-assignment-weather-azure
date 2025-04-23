using Assignment.AzureWeather.Domain.Models;

namespace Assignment.AzureWeather.Application.Interfaces;

public interface IWeatherInfoRepository
{
    Task AddAsync(WeatherInfo item);

    Task<IEnumerable<WeatherInfo>> GetAllAsync();
    
    Task<IEnumerable<WeatherInfo>> GetByDatesAsync(DateTime from, DateTime to);
}