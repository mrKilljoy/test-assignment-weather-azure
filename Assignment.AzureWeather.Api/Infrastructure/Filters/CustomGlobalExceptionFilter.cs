using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Assignment.AzureWeather.Api.Infrastructure.Filters;

public sealed class CustomGlobalExceptionFilter : IExceptionFilter
{
    private const string MessageText = "Unknown error";
    
    private readonly ILogger<CustomGlobalExceptionFilter> _logger;

    public CustomGlobalExceptionFilter(ILogger<CustomGlobalExceptionFilter> logger)
    {
        _logger = logger;
    }
    
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, MessageText);

        context.Result = new ObjectResult(new
        {
            Error = "An unexpected error has occurred."
        })
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
        
        context.ExceptionHandled = true;
    }
}