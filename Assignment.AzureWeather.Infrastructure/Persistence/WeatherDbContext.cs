using System.Reflection;
using Assignment.AzureWeather.Domain;
using Assignment.AzureWeather.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment.AzureWeather.Infrastructure.Persistence;

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }
    
    public DbSet<WeatherInfo> WeatherEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}