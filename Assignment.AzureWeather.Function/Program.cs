using Assignment.AzureWeather.Application.Interfaces;
using Assignment.AzureWeather.Infrastructure.Extensions;
using Assignment.AzureWeather.Infrastructure.Interfaces;
using Assignment.AzureWeather.Infrastructure.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddLogging();
        services.AddHttpClient();
        services.ApplyWeatherServiceSettings();
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<IWeatherLogService, WeatherLogService>();
    })
    .Build();

await host.RunAsync();