using Assignment.AzureWeather.Domain;
using Assignment.AzureWeather.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assignment.AzureWeather.Infrastructure.Persistence.Configuration;

public class WeatherInfoEntityConfiguration : IEntityTypeConfiguration<WeatherInfo>
{
    public void Configure(EntityTypeBuilder<WeatherInfo> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.City)
            .IsRequired();
        
        builder
            .Property(x => x.Country)
            .IsRequired();
    }
}