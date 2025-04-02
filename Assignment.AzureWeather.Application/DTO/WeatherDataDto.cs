namespace Assignment.AzureWeather.Application.DTO;

public class WeatherDataDto
{
    public string City { get; set; }

    public string Country { get; set; }

    public decimal Temperature { get; set; }

    public string RawData { get; set; }
}