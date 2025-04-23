using Assignment.AzureWeather.Application.Interfaces;
using Assignment.AzureWeather.Infrastructure.DTO.Requests;
using FluentValidation;
using FluentValidation.AspNetCore;
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
    public async Task<IActionResult> Get(
        [FromQuery]GetWeatherRequest request,
        [FromServices]IValidator<GetWeatherRequest> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            
            var result = request is { From: not null, To: not null } ?
                await _statisticsService.GetStatistics(request.From.Value, request.To.Value) :
                await _statisticsService.GetStatistics();
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