using Newtonsoft.Json;

namespace Assignment.AzureWeather.Infrastructure.DTO;

// todo: remove redundant data?
public class WeatherResponseDto
{
    //public object Coord { get; set; }
    //public Weather[] Weather { get; set; }
    //public string Base { get; set; }
    
    [JsonProperty("main")]
    public Main Main { get; set; }
    //public int Visibility { get; set; }
    //public object Wind { get; set; }
    //public object Clouds { get; set; }
    //public long Dt { get; set; }
    
    [JsonProperty("sys")]
    public Sys Sys { get; set; }
    //public int Timezone { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    //public int Cod { get; set; }
}