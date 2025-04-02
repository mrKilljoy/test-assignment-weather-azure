using Assignment.AzureWeather.Application.DTO;

namespace Assignment.AzureWeather.Infrastructure.Interfaces;

public interface IWeatherLogService
{
    Task LogAsync(bool isSuccess, string content = null);

    Task<string> GetLogPayloadAsync(string key);
    
    Task<IEnumerable<WeatherLogDto>> QueryAsync(DateTime from, DateTime to);
}