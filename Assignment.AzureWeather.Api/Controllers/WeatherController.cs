using Assignment.AzureWeather.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.AzureWeather.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WeatherController : ControllerBase
{
    private readonly ILogger<WeatherController> _logger;
    private readonly IWeatherStatisticsService _statisticsService;

    public WeatherController(ILogger<WeatherController> logger, IWeatherStatisticsService statisticsService)
    {
        _logger = logger;
        _statisticsService = statisticsService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var result = await _statisticsService.GetStatistics();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(Get));
            return Problem(detail: "An unexpected error has occurred.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}