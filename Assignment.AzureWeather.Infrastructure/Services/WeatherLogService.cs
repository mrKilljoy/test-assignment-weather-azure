using System.Text;
using Assignment.AzureWeather.Application.DTO;
using Assignment.AzureWeather.Infrastructure.Common;
using Assignment.AzureWeather.Infrastructure.Constants;
using Assignment.AzureWeather.Infrastructure.Interfaces;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Assignment.AzureWeather.Infrastructure.Services;

public class WeatherLogService : IWeatherLogService
{
    private readonly ILogger<WeatherLogService> _logger;
    private readonly IConfiguration _configuration;

    public WeatherLogService(ILogger<WeatherLogService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    
    public async Task LogAsync(bool isSuccess, string content = null)
    {
        try
        {
            DateTime now = DateTime.UtcNow;
            string status = isSuccess ? "Success" : "Failure";
            
            await CreateLogAsync(now, isSuccess);
            
            if (isSuccess)
            {
                await SavePayloadAsync(now, content);
            }
            
            _logger.LogInformation($"Logged attempt: {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(LogAsync));
            throw;
        }
    }

    public async Task<string> GetLogPayloadAsync(string key)
    {
        string connectionString = _configuration["AzureWebJobsStorage"];

        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(WeatherLogServiceConstants.ContainerName);
        BlobClient blobClient = containerClient.GetBlobClient(key);
        
        if (await blobClient.ExistsAsync())
        {
            var response = await blobClient.DownloadContentAsync();
            return response.Value?.Content?.ToString();
        }

        return null;
    }

    public async Task<IEnumerable<WeatherLogDto>> QueryAsync(DateTime from, DateTime to)
    {
        string connectionString = _configuration["AzureWebJobsStorage"];
        TableClient tableClient = new TableClient(connectionString, WeatherLogServiceConstants.TableName);

        var partitionKeys = BuildPartitionKeys(from, to);
        var items = new List<WeatherLogDto>();

        foreach (string pk in partitionKeys)
        {
            string filter = BuildTableFilterCondition(pk, from, to);
            var query = tableClient.QueryAsync<TableEntity>(filter);

            await foreach (var entity in query)
            {
                items.Add(new WeatherLogDto()
                {
                    Status = entity.GetString("Status"),
                    Created = entity.GetDateTime("Created") ?? DateTime.MinValue
                });
            }
        }

        return items.OrderByDescending(x => x.Created).ToList();
    }

    private async Task CreateLogAsync(DateTime date, bool isSuccess)
    {
        var connectionString = _configuration["AzureWebJobsStorage"];
        string partitionKey = date.ToString(WeatherLogServiceConstants.PartitionKeyFormat);
        string rowKey = date.ToString(WeatherLogServiceConstants.RowKeyFormat);
        
        TableClient tableClient = new TableClient(connectionString, WeatherLogServiceConstants.TableName);
        await tableClient.CreateIfNotExistsAsync();

        string status = isSuccess ? "Success" : "Failure";
        var entity = new TableEntity(partitionKey, rowKey)
        {
            { "Status", status },
            { "Created", date }
        };

        await tableClient.AddEntityAsync(entity);
    }

    private async Task SavePayloadAsync(DateTime date, string content)
    {
        var connectionString = _configuration["AzureWebJobsStorage"];
        string blobName = WeatherLogServiceHelper.BuildLogPayloadKey(date);
        
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(WeatherLogServiceConstants.ContainerName);
        await containerClient.CreateIfNotExistsAsync();
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await blobClient.UploadAsync(stream, overwrite: true);
    }

    private List<string> BuildPartitionKeys(DateTime from, DateTime to)
    {
        List<string> partitionKeys = new List<string>();
        for (DateTime date = from.Date; date <= to.Date; date = date.AddDays(1))
        {
            partitionKeys.Add(date.ToString(WeatherLogServiceConstants.PartitionKeyFormat));
        }

        return partitionKeys;
    }

    private string BuildTableFilterCondition(string partitionKey, DateTime from, DateTime to)
    {
        string rowKeyStart = partitionKey == from.ToString(WeatherLogServiceConstants.PartitionKeyFormat) ?
            from.ToString(WeatherLogServiceConstants.RowKeyFormat) :
            "000000";
        string rowKeyEnd = partitionKey == to.ToString(WeatherLogServiceConstants.PartitionKeyFormat) ?
            to.ToString(WeatherLogServiceConstants.RowKeyFormat) :
            "235959";

        return $"PartitionKey eq '{partitionKey}' and RowKey ge '{rowKeyStart}' and RowKey le '{rowKeyEnd}'";
    }
}