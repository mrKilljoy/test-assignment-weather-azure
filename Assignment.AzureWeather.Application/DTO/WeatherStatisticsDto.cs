namespace Assignment.AzureWeather.Application.DTO;

public class WeatherStatisticsDto
{
    public string City { get; set; }

    public string Country { get; set; }

    public decimal Temperature { get; set; }

    public DateTime DateCreated { get; set; }
}