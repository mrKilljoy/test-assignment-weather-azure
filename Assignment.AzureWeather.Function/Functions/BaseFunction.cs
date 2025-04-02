using System.Net;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace Assignment.AzureWeather.Function.Functions;

public abstract class BaseFunction
{
    protected async Task<HttpResponseData> CreateJsonResponseAsync(
        HttpRequestData requestData,
        HttpStatusCode statusCode,
        object content)
    {
        var response = requestData.CreateResponse(statusCode);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonConvert.SerializeObject(content));
        
        return response;
    }

    protected async Task<HttpResponseData> CreateResponseAsync(
        HttpRequestData requestData,
        HttpStatusCode statusCode,
        string message = null)
    {
        var response = requestData.CreateResponse(statusCode);
        response.Headers.Add("Content-Type", "text/plain");

        if (!string.IsNullOrEmpty(message))
            await response.WriteStringAsync(message);
        
        return response;
    }
}