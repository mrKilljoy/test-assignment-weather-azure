using System.Net;
using Assignment.AzureWeather.Infrastructure.Common;
using Assignment.AzureWeather.Infrastructure.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Assignment.AzureWeather.Function.Functions;

public class GetPayloadFunction : BaseFunction
{
    private readonly ILogger _logger;
    private readonly IWeatherLogService _weatherLogService;

    public GetPayloadFunction(ILogger<GetPayloadFunction> logger, IWeatherLogService weatherLogService)
    {
        _logger = logger;
        _weatherLogService = weatherLogService;
    }

    [Function("GetPayload")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payload/{date}/{time}")] HttpRequestData req,
        string date,    // Should have 'yyyyMMdd' format
        string time)    // Should have 'HHmmss' format
    {
        try
        {
            _logger.LogInformation($"Retrieving payload from {date}-{time}...");

            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(time))
            {
                return await CreateResponseAsync(
                    req,
                    HttpStatusCode.BadRequest,
                    "Please provide date and time when the payload was saved.");
            }

            string key = WeatherLogServiceHelper.BuildLogPayloadKey(date, time);
            var result = await _weatherLogService.GetLogPayloadAsync(key);

            return string.IsNullOrEmpty(result) ?
                await CreateResponseAsync(req, HttpStatusCode.NotFound) :
                await CreateJsonResponseAsync(req, HttpStatusCode.OK, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error has occurred.");
            return await CreateResponseAsync(req, HttpStatusCode.InternalServerError);
        }
    }
}