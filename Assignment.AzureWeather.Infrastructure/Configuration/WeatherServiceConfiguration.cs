namespace Assignment.AzureWeather.Infrastructure.Configuration;

public class WeatherServiceConfiguration
{
    internal const string SectionName = "WeatherService";
    
    public string Units { get; set; }

    public string ApiKey { get; set; }

    public string ServiceUrl { get; set; }
}