using Assignment.AzureWeather.Application.Interfaces;
using Assignment.AzureWeather.Domain.Models;
using Assignment.AzureWeather.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Assignment.AzureWeather.Infrastructure.Services;

public class WeatherInfoRepository : IWeatherInfoRepository
{
    private readonly WeatherDbContext _dbContext;

    public WeatherInfoRepository(WeatherDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAsync(WeatherInfo item)
    {
        await _dbContext.WeatherEntries.AddAsync(item);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<WeatherInfo>> GetAllAsync()
    {
        return await _dbContext.WeatherEntries.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<WeatherInfo>> GetByDatesAsync(DateTime from, DateTime to)
    {
        return await _dbContext.WeatherEntries
            .AsNoTracking()
            .Where(x => x.DateCreated.Date >= from.Date && x.DateCreated.Date <= to.Date)
            .ToListAsync();
    }
}