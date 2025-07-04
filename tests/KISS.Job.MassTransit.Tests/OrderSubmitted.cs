namespace KISS.Job.MassTransit.Tests;

public record OrderSubmitted
{
    public Guid OrderId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
}