namespace KISS.Job.MassTransit;

/// <summary>
/// Provides extension methods for registering MassTransit consumers, producers, and processors in the dependency injection container.
/// </summary>
public static class ServiceRegistrationExtensions
{
    /// <summary>
    /// Registers a MassTransit consumer for the specified message type and configures a RabbitMQ receive endpoint.
    /// </summary>
    /// <typeparam name="T">The type of the message payload to consume.</typeparam>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="sectionName">The configuration section name for RabbitMq settings (default: "RabbitMq").</param>
    /// <param name="queueName">The name of the RabbitMQ queue (default: "default").</param>
    public static void UseConsumer<T>(this IServiceCollection services, IConfiguration configuration, string sectionName = "RabbitMq", string queueName = "default")
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);

        ArgumentNullException.ThrowIfNull(section);

        // Bind Redis configuration options and validate on startup
        services
            .AddOptions<RabbitMqConfigurationOption>()
            .Bind(section)
            .ValidateOnStart();

        services.AddMassTransit(x =>
            {
                x.AddConsumer<TaskConsumer<T>>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    var options = context.GetRequiredService<IOptions<RabbitMqConfigurationOption>>().Value;

                    cfg.Host(options.Host, options.Port, options.VirtualHost, h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                        if (options.UseSsl)
                        {
                            h.UseSsl(s => { });
                        }

                        h.Heartbeat(options.HeartbeatSeconds);
                    });

                    cfg.ReceiveEndpoint(queueName ?? $"{queueName}-queue", e =>
                    {
                        e.PrefetchCount = 1; // Process one message at a time
                        e.ConfigureConsumer<TaskConsumer<T>>(context);
                    });
                });
            });
    }

    /// <summary>
    /// Registers a producer for publishing task messages of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the payload to produce.</typeparam>
    /// <param name="services">The service collection to register with.</param>
    public static void UseProducer<T>(this IServiceCollection services)
    {
        services.AddSingleton<ITaskProducer<T>, TaskProducer<T>>();
    }

    /// <summary>
    /// Registers a task processor implementation for the specified payload type.
    /// </summary>
    /// <typeparam name="T">The type of the payload to process.</typeparam>
    /// <typeparam name="TProcessor">The processor implementation type.</typeparam>
    /// <param name="services">The service collection to register with.</param>
    public static void UseTaskProcessor<T, TProcessor>(this IServiceCollection services)
        where TProcessor : class, ITaskProcessor<T>
    {
        services.AddSingleton<ITaskProcessor<T>, TProcessor>();
    }
}