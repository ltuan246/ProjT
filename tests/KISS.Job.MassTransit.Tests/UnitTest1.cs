namespace KISS.Job.MassTransit.Tests;

public class UnitTest1 : IDisposable
{
    private ServiceProvider Services { get; init; }
    private IBusControl BusControl { get; init; }
    private ITaskProcessor<OrderSubmitted> OrderProcessor { get; init; }
    private ITaskProducer<OrderSubmitted> Producer { get; init; }

    public UnitTest1()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        services.UseTaskProcessor<OrderSubmitted, OrderTaskProcessor>();
        services.UseProducer<OrderSubmitted>();
        services.UseConsumer<OrderSubmitted>(configuration);

        Services = services.BuildServiceProvider();

        BusControl = Services.GetRequiredService<IBusControl>();
        OrderProcessor = Services.GetRequiredService<ITaskProcessor<OrderSubmitted>>();
        Producer = Services.GetRequiredService<ITaskProducer<OrderSubmitted>>();
        BusControl.StartAsync().GetAwaiter().GetResult(); // Start bus once
    }

    [Fact]
    public async Task EnqueueAsync_WhenOrderSubmittedMessagePublished_OrderIsProcessed()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var payload = new OrderSubmitted { OrderId = orderId, CustomerName = "Integration Test Customer" };

        // Act
        await Producer.EnqueueAsync(payload);

        // Assert
        await Task.Delay(1000); // Wait for message processing
        Assert.True(((OrderTaskProcessor)OrderProcessor).HasProcessedOrder(orderId), "Order should have been processed");
    }

    [Fact]
    public async Task Consume_WithInMemoryTestHarness_ProcessesOrderMessage()
    {
        // Arrange
        var processor = new OrderTaskProcessor();
        var harness = new InMemoryTestHarness();
        harness.Consumer(() => new TaskConsumer<OrderSubmitted>(processor));

        await harness.Start();
        try
        {
            var orderId = Guid.NewGuid();
            var payload = new OrderSubmitted { OrderId = orderId, CustomerName = "Test Customer" };
            var message = new TaskMessage<OrderSubmitted> { Payload = payload };

            // Act
            await harness.InputQueueSendEndpoint.Send(message);

            // Assert
            Assert.True(await harness.Consumed.Any<TaskMessage<OrderSubmitted>>(x => x.Context.Message.Payload.OrderId == orderId));
            Assert.True(processor.HasProcessedOrder(orderId), "Order should have been processed");
        }
        finally
        {
            await harness.Stop();
        }
    }

    public void Dispose()
    {
        BusControl.StopAsync().GetAwaiter().GetResult();
        Services.DisposeAsync().GetAwaiter().GetResult();
    }
}

// Payload type
public record OrderSubmitted
{
    public Guid OrderId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
}

// Task processor implementation
public class OrderTaskProcessor : ITaskProcessor<OrderSubmitted>
{
    private ConcurrentBag<OrderSubmitted> ProcessedOrders { get; } = new();

    public async Task ProcessAsync(OrderSubmitted payload)
    {
        ProcessedOrders.Add(payload);
        Console.WriteLine($"Processed order {payload.OrderId} for {payload.CustomerName}");
        await Task.CompletedTask;
    }

    // For test assertions
    public bool HasProcessedOrder(Guid orderId) =>
        ProcessedOrders.Any(o => o.OrderId == orderId);
}