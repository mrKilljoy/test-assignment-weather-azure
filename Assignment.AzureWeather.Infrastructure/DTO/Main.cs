﻿namespace Assignment.AzureWeather.Infrastructure.DTO;

// todo: remove redundant fields?
public class Main
{
    public decimal Temp { get; set; }
    public decimal Feels_Like { get; set; }
    public decimal Temp_Min { get; set; }
    public decimal Temp_Max { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public int Sea_Level { get; set; }
    public int Grnd_Level { get; set; }
}