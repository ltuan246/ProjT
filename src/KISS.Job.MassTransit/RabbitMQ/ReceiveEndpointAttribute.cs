namespace KISS.Job.MassTransit.RabbitMQ;

/// <summary>
/// Attribute for specifying the RabbitMQ receive endpoint (queue name) for a consumer class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ReceiveEndpointAttribute(string queueName) : Attribute
{
    /// <summary>
    /// Gets the name of the RabbitMQ queue associated with the consumer.
    /// </summary>
    public string QueueName { get; } = queueName;
}