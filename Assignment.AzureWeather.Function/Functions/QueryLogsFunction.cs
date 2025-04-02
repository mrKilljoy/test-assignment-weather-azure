using System.Net;
using Assignment.AzureWeather.Infrastructure.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Assignment.AzureWeather.Function.Functions;

public class QueryLogsFunction : BaseFunction
{
    private readonly IWeatherLogService _weatherLogService;
    private readonly ILogger _logger;

    public QueryLogsFunction(ILoggerFactory loggerFactory, IWeatherLogService weatherLogService)
    {
        _weatherLogService = weatherLogService;
        _logger = loggerFactory.CreateLogger<QueryLogsFunction>();
    }

    [Function("QueryLogs")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "logs")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        try
        {
            _logger.LogInformation("Querying logs...");

            var (from, to, error) = await ValidateInputAsync(req);

            if (error is not null)
                return error;

            var result = await _weatherLogService.QueryAsync(from, to);
            
            _logger.LogInformation("Logs received.");

            return await CreateJsonResponseAsync(req, HttpStatusCode.OK, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error has occurred");
            return await CreateResponseAsync(req, HttpStatusCode.InternalServerError);
        }
    }

    private async Task<(DateTime from, DateTime to, HttpResponseData error)> ValidateInputAsync(HttpRequestData requestData)
    {
        string from = requestData.Query["from"];
        string to = requestData.Query["to"];

        if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            return (default, default, await CreateResponseAsync(requestData, HttpStatusCode.BadRequest, "No valid date range provided."));

        if (!DateTime.TryParse(from, out DateTime fromDate) || !DateTime.TryParse(to, out DateTime toDate))
            return (default, default, await CreateResponseAsync(requestData, HttpStatusCode.BadRequest, "Invalid date format for 'from' or 'to'."));

        if (fromDate > toDate)
            return (default, default, await CreateResponseAsync(requestData, HttpStatusCode.BadRequest, "Invalid date range."));

        return (fromDate, toDate, null);
    }
}