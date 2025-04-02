namespace Assignment.AzureWeather.Domain.Models;

public class WeatherInfo
{
    public Guid Id { get; set; }
    
    public string City { get; set; }

    public string Country { get; set; }

    public decimal Temperature { get; set; }

    public DateTime DateCreated { get; set; }
}