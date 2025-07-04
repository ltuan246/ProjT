namespace KISS.Job.MassTransit.Tests;

[ReceiveEndpoint("order-submitted")]
public class OrderService : IConsumer<OrderSubmitted>
{
    public async Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        await Task.Delay(1000);
        await Task.CompletedTask; // Simulate some processing delay
    }
}