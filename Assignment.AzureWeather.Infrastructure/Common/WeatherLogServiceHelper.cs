using Assignment.AzureWeather.Infrastructure.Constants;

namespace Assignment.AzureWeather.Infrastructure.Common;

public static class WeatherLogServiceHelper
{
    /// <summary>
    /// Build a key for retrieving payload for a specific weather request.
    /// </summary>
    /// <param name="date">The date when the request was made. Used as a partition key.</param>
    /// <param name="time">The time when the request was made. Used as a row key.</param>
    /// <returns>A name of a blob object from Azure Storage.</returns>
    /// <exception cref="Exception"></exception>
    public static string BuildLogPayloadKey(string date, string time)
    {
        if (string.IsNullOrEmpty(date))
            throw new ArgumentNullException(nameof(date));
        
        if (string.IsNullOrEmpty(time))
            throw new ArgumentNullException(nameof(time));
        
        return $"weather-{date}-{time}.json";
    }

    /// <summary>
    /// Build a key for retrieving payload for a specific weather request.
    /// </summary>
    /// <param name="date">The date when the request was made.</param>
    /// <returns>A name of a blob object from Azure Storage.</returns>
    public static string BuildLogPayloadKey(DateTime date)
    {
        string partitionKey = date.ToString(WeatherLogServiceConstants.PartitionKeyFormat);
        string rowKey = date.ToString(WeatherLogServiceConstants.RowKeyFormat);
        
        return $"weather-{partitionKey}-{rowKey}.json";
    }
}