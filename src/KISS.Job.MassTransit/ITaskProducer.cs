namespace KISS.Job.MassTransit;

/// <summary>
/// Defines a contract for producing and enqueuing task messages for processing.
/// </summary>
/// <typeparam name="T">The type of the payload to enqueue.</typeparam>
public interface ITaskProducer<T>
{
    /// <summary>
    /// Asynchronously enqueues a payload for processing as a task message.
    /// </summary>
    /// <param name="payload">The payload to enqueue.</param>
    /// <returns>A task representing the asynchronous enqueue operation.</returns>
    Task EnqueueAsync(T payload);
}