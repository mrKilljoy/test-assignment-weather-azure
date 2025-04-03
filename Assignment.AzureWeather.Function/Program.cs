using Assignment.AzureWeather.Application.Interfaces;
using Assignment.AzureWeather.Infrastructure.Extensions;
using Assignment.AzureWeather.Infrastructure.Interfaces;
using Assignment.AzureWeather.Infrastructure.Services;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddAzureClients(x =>
        {
            var connectionString = context.Configuration["AzureWebJobsStorage"];
            x.AddTableServiceClient(connectionString);
            x.AddBlobServiceClient(connectionString);
        });
        services.AddLogging();
        services.AddHttpClient();
        services.ApplyWeatherServiceSettings();
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<IWeatherLogService, WeatherLogService>();
    })
    .Build();

await host.RunAsync();