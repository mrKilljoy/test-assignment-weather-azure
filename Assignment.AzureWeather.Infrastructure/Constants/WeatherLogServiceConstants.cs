namespace Assignment.AzureWeather.Infrastructure.Constants;

public static class WeatherLogServiceConstants
{
    public const string PartitionKeyFormat = "yyyyMMdd";
    
    public const string RowKeyFormat = "HHmmss";
    
    public const string TableName = "WeatherLogs";
    
    public const string ContainerName = "weatherdata";
}