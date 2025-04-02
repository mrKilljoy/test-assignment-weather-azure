namespace Assignment.AzureWeather.Infrastructure.Configuration;

public class WeatherWorkerConfiguration
{
    public const string SectionName = "WeatherWorker";
    
    public List<string> Locations { get; set; }

    public int IntervalSeconds { get; set; }
}