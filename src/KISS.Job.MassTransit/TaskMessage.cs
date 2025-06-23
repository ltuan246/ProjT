namespace KISS.Job.MassTransit;

/// <summary>
/// Represents a generic task message for use with MassTransit, containing a unique task ID, payload, and timestamp.
/// </summary>
/// <typeparam name="T">The type of the payload carried by the message.</typeparam>
public sealed record TaskMessage<T>
{
    /// <summary>
    /// Gets the unique identifier for the task message.
    /// </summary>
    public Guid TaskId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the payload of the task message.
    /// </summary>
    public required T Payload { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the message was created.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}