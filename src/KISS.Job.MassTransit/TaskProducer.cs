namespace KISS.Job.MassTransit;

/// <summary>
/// Produces and publishes <see cref="TaskMessage{T}"/> messages to the message bus using MassTransit.
/// </summary>
/// <typeparam name="T">The type of the payload to enqueue and publish.</typeparam>
public sealed record TaskProducer<T>(IPublishEndpoint PublishEndpoint) : ITaskProducer<T>, IAsyncDisposable
{
    /// <summary>
    /// Gets a value indicating whether the producer has been disposed.
    /// </summary>
    private bool IsDisposed { get; set; }

    /// <summary>
    /// Asynchronously enqueues a payload by publishing a <see cref="TaskMessage{T}"/> to the message bus.
    /// </summary>
    /// <param name="payload">The payload to enqueue and publish.</param>
    /// <returns>A task representing the asynchronous publish operation.</returns>
    public async Task EnqueueAsync(T payload)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, nameof(TaskProducer<T>));

        var message = new TaskMessage<T>
        {
            Payload = payload
        };

        await PublishEndpoint.Publish(message);
    }

    /// <summary>
    /// Asynchronously disposes the producer, marking it as disposed.
    /// </summary>
    /// <returns>A completed ValueTask representing the dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
        }

        await Task.CompletedTask;
    }
}