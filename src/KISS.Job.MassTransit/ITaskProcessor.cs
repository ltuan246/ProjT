namespace KISS.Job.MassTransit;

/// <summary>
/// Defines a contract for processing task messages with a specific payload type.
/// </summary>
/// <typeparam name="T">The type of the payload to process.</typeparam>
public interface ITaskProcessor<T>
{
    /// <summary>
    /// Asynchronously processes the provided payload as a task message.
    /// </summary>
    /// <param name="payload">The payload to process.</param>
    /// <returns>A task representing the asynchronous processing operation.</returns>
    Task ProcessAsync(T payload);
}