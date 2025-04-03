using Assignment.AzureWeather.Api.Infrastructure.Constants;
using Assignment.AzureWeather.Api.Infrastructure.Extensions;
using Assignment.AzureWeather.Api.Infrastructure.Filters;
using Assignment.AzureWeather.Api.Services;
using Assignment.AzureWeather.Infrastructure.Extensions;
using Assignment.AzureWeather.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Assignment.AzureWeather.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLogging(x => x.AddDebug());
        builder.Services.AddHttpClient();
        
        builder.Services.ApplyWeatherWorkerSettings();
        builder.Services.ApplyWeatherServiceSettings();
        
        builder.Services.AddHostedService<WeatherBackgroundService>();

        builder.Services.AddPersistence(builder.Configuration);

        builder.Services.RegisterApiDependencies();
        
        builder.Services.AddControllers(x => x.Filters.Add<CustomGlobalExceptionFilter>());
        builder.Services.AddCustomCorsPolicy(builder.Configuration);
        
        var app = builder.Build();
        
        app.UseRouting();
        app.UseCors(ApiConstants.CustomCorsPolicyName);
        app.MapControllers();

        await app.RunAsync();
    }
}