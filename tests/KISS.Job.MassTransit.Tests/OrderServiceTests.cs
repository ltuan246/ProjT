
namespace KISS.Job.MassTransit.Tests;

public class RequestResponseServiceTests : IAsyncLifetime
{
    private IBusControl BusControl { get; init; }

    public RequestResponseServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        ServiceCollection services = new();
        services.UseRabbitMq(configuration);

        var provider = services.BuildServiceProvider();
        BusControl = provider.GetRequiredService<IBusControl>();
    }

    public async Task InitializeAsync()
    {
        await BusControl.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await BusControl.StopAsync();
    }

    [Fact]
    public async Task Consume_OrderSubmittedMessage_ConsumedSuccessfully()
    {
        // Arrange
        const string queueName = "order-submitted";
        var message = new OrderSubmitted { OrderId = Guid.NewGuid(), CustomerName = "Integration Test Customer" };

        // Act: Send message to the specific queue
        try
        {
            var sendEndpoint = await BusControl.GetSendEndpoint(new Uri($"queue:{queueName}"));
            await sendEndpoint.Send(message);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Send failed: {ex.Message}");
        }

        // Assert: Verify message reached the queue
        var messageInQueue = await CheckQueueMessageCount(queueName, expectedCount: 1);
        Assert.True(messageInQueue);

        // Assert: Verify consumption
        await Task.Delay(2000); // Wait for async processing

        // Assert: Verify queue is empty after consumption
        var queueEmpty = await CheckQueueMessageCount(queueName, expectedCount: 0);
        Assert.True(queueEmpty);
    }

    private async Task<bool> CheckQueueMessageCount(string queueName, int expectedCount)
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("guest:guest")));
            var response = await client.GetAsync($"http://localhost:15672/api/queues/%2F/{queueName}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var queueInfo = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            if (queueInfo!.TryGetValue("messages", out var messages))
            {
                return (int)messages == expectedCount;
            }

            return false;
        }
        catch
        {
            return false; // Queue may not exist (e.g., error queue before fault)
        }
    }
}