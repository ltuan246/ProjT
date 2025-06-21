namespace KISS.Caching.Utils;

/// <summary>
///     Provides a background processor for handling asynchronous tasks sequentially.
/// </summary>
public sealed class BackgroundTaskProcessor : IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BackgroundTaskProcessor" /> class and starts processing tasks in the
    ///     background.
    /// </summary>
    public BackgroundTaskProcessor() => Task.Run(ProcessQueueAsync);

    /// <summary>
    ///     The queue of tasks to be processed.
    /// </summary>
    private ConcurrentQueue<Func<Task>> Queue { get; } = new();

    /// <summary>
    ///     Indicates whether the processor has been disposed.
    /// </summary>
    private bool IsDisposed { get; set; }

    /// <summary>
    ///     Releases resources used by the <see cref="BackgroundTaskProcessor" />.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Enqueues a new asynchronous task for background processing.
    /// </summary>
    /// <param name="task">A delegate representing the asynchronous task to enqueue.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the processor has been disposed.</exception>
    public void Enqueue(Func<Task> task)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, nameof(BackgroundTaskProcessor));

        Queue.Enqueue(task);
    }

    /// <summary>
    ///     Continuously processes tasks from the queue until the processor is disposed.
    /// </summary>
    private async Task ProcessQueueAsync()
    {
        while (!IsDisposed)
        {
            if (Queue.TryDequeue(out var task))
            {
                try
                {
                    await task();
                }
                catch (OperationCanceledException)
                {
                    // Task was cancelled, handle if necessary
                }
            }
            else
            {
                await Task.Delay(100); // Prevent tight loop
            }
        }
    }

    /// <summary>
    ///     Releases the unmanaged resources used by the <see cref="BackgroundTaskProcessor" /> and optionally releases the
    ///     managed resources.
    /// </summary>
    /// <param name="disposing">
    ///     True to release both managed and unmanaged resources; false to release only unmanaged
    ///     resources.
    /// </param>
    private void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                // Release managed resources if any
            }

            // Release unmanaged resources if any
            IsDisposed = true;
        }
    }
}
