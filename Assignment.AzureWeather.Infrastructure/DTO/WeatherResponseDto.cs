using Newtonsoft.Json;

namespace Assignment.AzureWeather.Infrastructure.DTO;

public class WeatherResponseDto
{
    [JsonProperty("main")]
    public Main Main { get; set; }
    
    [JsonProperty("sys")]
    public Sys Sys { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
}