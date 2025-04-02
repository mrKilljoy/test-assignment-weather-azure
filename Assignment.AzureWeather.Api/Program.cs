using Assignment.AzureWeather.Api.Infrastructure.Filters;
using Assignment.AzureWeather.Api.Services;
using Assignment.AzureWeather.Application.Interfaces;
using Assignment.AzureWeather.Infrastructure.Extensions;
using Assignment.AzureWeather.Infrastructure.Persistence;
using Assignment.AzureWeather.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Assignment.AzureWeather.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLogging(x => x.AddDebug());
        builder.Services.AddHttpClient();
        builder.Services.ApplyLocationSettings();
        builder.Services.ApplyWeatherServiceSettings();
        builder.Services.AddHostedService<WeatherBackgroundService>();

        builder.Services.AddDbContext<WeatherDbContext>(x => x.UseInMemoryDatabase("test-db")); // todo: check

        builder.Services.AddTransient<IWeatherService, WeatherService>();
        builder.Services.AddTransient<IWeatherInfoRepository, WeatherInfoRepository>();
        builder.Services.AddTransient<IWeatherStatisticsService, WeatherStatisticsService>();
        
        builder.Services.AddControllers(x => x.Filters.Add<CustomGlobalExceptionFilter>());
        
        var app = builder.Build();
        
        app.UseRouting();
        app.MapControllers();

        await app.RunAsync();
    }
}