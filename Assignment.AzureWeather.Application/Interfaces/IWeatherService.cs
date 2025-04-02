using Assignment.AzureWeather.Application.DTO;

namespace Assignment.AzureWeather.Application.Interfaces;

public interface IWeatherService
{
    Task<WeatherDataDto> GetCurrentWeatherAsync(string location);
}