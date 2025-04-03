using Assignment.AzureWeather.Infrastructure.Services;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Assignment.AzureWeather.Tests.Services;

[TestFixture]
public class WeatherLogServiceTests
{
    private Mock<ILogger<WeatherLogService>> _loggerMock;
    private Mock<BlobServiceClient> _blobServiceClientMock;
    private Mock<TableServiceClient> _tableServiceClientMock;
    private WeatherLogService _weatherLogService;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<WeatherLogService>>();
        _blobServiceClientMock = new Mock<BlobServiceClient>();
        _tableServiceClientMock = new Mock<TableServiceClient>();

        _weatherLogService = new WeatherLogService(
            _loggerMock.Object,
            _blobServiceClientMock.Object,
            _tableServiceClientMock.Object
        );
    }

    [Test]
    public async Task LogAsync_SuccessfulLog_ShouldLogInformation()
    {
        var blobClientMock = new Mock<BlobClient>();
        var blobContainerClientMock = new Mock<BlobContainerClient>();
        var tableClientMock = new Mock<TableClient>();

        _blobServiceClientMock
            .Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(blobContainerClientMock.Object);

        blobClientMock
            .Setup(x => x.ExistsAsync(CancellationToken.None))
            .ReturnsAsync(Response.FromValue(true, null));

        blobContainerClientMock
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);

        _tableServiceClientMock
            .Setup(x => x.GetTableClient(It.IsAny<string>()))
            .Returns(tableClientMock.Object);

        await _weatherLogService.LogAsync(true, "Sample content");

        _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Test]
    public async Task GetLogPayloadAsync_ExistingBlob_ReturnsContent()
    {
        var blobContainerClientMock = new Mock<BlobContainerClient>();
        var blobClientMock = new Mock<BlobClient>();

        _blobServiceClientMock
            .Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(blobContainerClientMock.Object);

        blobClientMock
            .Setup(x => x.ExistsAsync(CancellationToken.None))
            .ReturnsAsync(Response.FromValue(true, null));

        blobContainerClientMock
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);

        var responseMock = BlobsModelFactory.BlobDownloadResult(content: BinaryData.FromString("Test content"));

        blobClientMock
            .Setup(x => x.DownloadContentAsync())
            .ReturnsAsync(Response.FromValue(responseMock, null));

        var result = await _weatherLogService.GetLogPayloadAsync("test-key");

        Assert.That(result, Is.EqualTo("Test content"));
    }

    [Test]
    public async Task GetLogPayloadAsync_NonExistingBlob_ReturnsNull()
    {
        var blobContainerClientMock = new Mock<BlobContainerClient>();
        var blobClientMock = new Mock<BlobClient>();

        _blobServiceClientMock
            .Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(blobContainerClientMock.Object);

        blobContainerClientMock
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClientMock.Object);

        blobClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(false, null));

        var result = await _weatherLogService.GetLogPayloadAsync("test-key");

        Assert.IsNull(result);
    }

    [Test]
    public async Task QueryAsync_ReturnsExpectedLogs()
    {
        var tableClientMock = new Mock<TableClient>();
        _tableServiceClientMock
            .Setup(x => x.GetTableClient(It.IsAny<string>()))
            .Returns(tableClientMock.Object);

        var testEntities = new List<TableEntity>
        {
            new TableEntity("Partition1", "Row1") { { "Status", "Success" }, { "Created", DateTime.UtcNow.AddDays(-1) } },
            new TableEntity("Partition1", "Row2") { { "Status", "Failure" }, { "Created", DateTime.UtcNow } }
        };

        var asyncPageableMock = CreateAsyncPageableMock(testEntities);

        tableClientMock
            .Setup(x => x.QueryAsync<TableEntity>(It.IsAny<string>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(asyncPageableMock.Object);

        var result = await _weatherLogService.QueryAsync(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow);

        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.IsTrue(result.Any(x => x.Status == "Success"));
        Assert.IsTrue(result.Any(x => x.Status == "Failure"));
    }

    private Mock<AsyncPageable<TableEntity>> CreateAsyncPageableMock(IEnumerable<TableEntity> entities)
    {
        var pageableMock = new Mock<AsyncPageable<TableEntity>>();

        pageableMock
            .Setup(p => p.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<TableEntity>(entities));

        return pageableMock;
    }

    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public TestAsyncEnumerator(IEnumerable<T> enumerable)
        {
            _enumerator = enumerable.GetEnumerator();
        }

        public T Current => _enumerator.Current;

        public ValueTask DisposeAsync()
        {
            _enumerator.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_enumerator.MoveNext());
    }
}