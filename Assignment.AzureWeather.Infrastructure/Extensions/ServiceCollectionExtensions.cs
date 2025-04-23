using Assignment.AzureWeather.Application.Interfaces;
using Assignment.AzureWeather.Infrastructure.Configuration;
using Assignment.AzureWeather.Infrastructure.DTO.Requests;
using Assignment.AzureWeather.Infrastructure.Services;
using Assignment.AzureWeather.Infrastructure.Validation;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Assignment.AzureWeather.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ApplyWeatherServiceSettings(this IServiceCollection services)
    {
        services.AddOptions<WeatherServiceConfiguration>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration
                    .GetSection(WeatherServiceConfiguration.SectionName)
                    .Bind(settings);
            });

        return services;
    }
    
    public static IServiceCollection ApplyWeatherWorkerSettings(this IServiceCollection services)
    {
        services.AddOptions<WeatherWorkerConfiguration>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration
                    .GetSection(WeatherWorkerConfiguration.SectionName)
                    .Bind(settings);
            });

        return services;
    }

    public static IServiceCollection RegisterApiDependencies(this IServiceCollection services)
    {
        services.AddTransient<IWeatherService, WeatherService>();
        services.AddTransient<IWeatherInfoRepository, WeatherInfoRepository>();
        services.AddTransient<IWeatherStatisticsService, WeatherStatisticsService>();
        services.AddTransient<IValidator<GetWeatherRequest>, DateRangeValidator>();

        return services;
    }
}