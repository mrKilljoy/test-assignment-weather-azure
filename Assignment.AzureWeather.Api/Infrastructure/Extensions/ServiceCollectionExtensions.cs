using Assignment.AzureWeather.Api.Infrastructure.Constants;
using Assignment.AzureWeather.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Assignment.AzureWeather.Api.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var frontUrl = configuration[ApiConstants.FrontUrlProperty];

        if (string.IsNullOrEmpty(frontUrl))
            return services;

        services.AddCors(
            options => options.AddPolicy(
                ApiConstants.CustomCorsPolicyName,
                policy => policy.WithOrigins([frontUrl])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ApiConstants.DatabaseConnectionStringName);
        services.AddDbContext<WeatherDbContext>(x => x.UseSqlServer(connectionString));

        return services;
    }
}