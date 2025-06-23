namespace KISS.Job.MassTransit;

/// <summary>
/// Consumes <see cref="TaskMessage{T}"/> messages and delegates processing to an <see cref="ITaskProcessor{T}"/> implementation.
/// </summary>
/// <typeparam name="T">The type of the payload contained in the task message.</typeparam>
public sealed record TaskConsumer<T>(ITaskProcessor<T> TaskProcessor) : IConsumer<TaskMessage<T>>
{
    /// <summary>
    /// Consumes a <see cref="TaskMessage{T}"/> from the message bus and processes its payload.
    /// </summary>
    /// <param name="context">The MassTransit consume context containing the task message.</param>
    /// <returns>A task representing the asynchronous consume operation.</returns>
    public async Task Consume([NotNull] ConsumeContext<TaskMessage<T>> context)
    {
        var message = context.Message;

        try
        {
            await TaskProcessor.ProcessAsync(message.Payload);
        }
        catch (Exception ex)
        {
            _ = ex;
            throw; // Let MassTransit handle retries or dead-lettering
        }
    }
}
