namespace Assignment.AzureWeather.Infrastructure.Exceptions;

[Serializable]
public class ConfigurationItemNotFoundException : Exception
{
    public ConfigurationItemNotFoundException(string itemName) : base()
    {
        ItemName = itemName;
    }

    public string ItemName { get; set; }
}