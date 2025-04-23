namespace Assignment.AzureWeather.Infrastructure.DTO.Requests;

public class GetWeatherRequest
{
    public DateTime? From { get; set; }
    
    public DateTime? To { get; set; }
}