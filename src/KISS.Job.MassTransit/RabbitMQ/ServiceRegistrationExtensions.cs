namespace KISS.Job.MassTransit.RabbitMQ;

/// <summary>
/// Provides extension methods for registering RabbitMQ consumers and endpoints in the dependency injection container.
/// </summary>
public static class ServiceRegistrationExtensions
{
    /// <summary>
    /// Registers all consumers decorated with <see cref="ReceiveEndpointAttribute"/> and configures RabbitMQ endpoints for them using configuration options.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="sectionName">The configuration section name for RabbitMQ settings (default: "RabbitMq").</param>
    public static void UseRabbitMq(this IServiceCollection services, IConfiguration configuration, string sectionName = "RabbitMq")
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);

        ArgumentNullException.ThrowIfNull(section);

        // Bind RabbitMQ configuration options and validate on startup
        services
            .AddOptions<RabbitMqConfigurationOption>()
            .Bind(section)
            .ValidateOnStart();

        var targetServices = GetServices();

        services.AddMassTransit(x =>
        {
            // Register each consumer with its corresponding queue name
            foreach (var (_, implementationType) in targetServices)
            {
                x.AddConsumer(implementationType);
            }

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

                // Register endpoints for all discovered consumers
                cfg.UseEndpoints(context, targetServices);
            });
        });
    }

    /// <summary>
    /// Configures receive endpoints for each discovered consumer and attaches them to the RabbitMQ bus.
    /// </summary>
    /// <param name="cfg">The RabbitMQ bus factory configurator.</param>
    /// <param name="context">The bus registration context.</param>
    /// <param name="targetServices">The discovered queue name and implementation type pairs.</param>
    private static void UseEndpoints(this IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context, IEnumerable<(string QueueName, Type ImplementationType)> targetServices)
    {
        foreach (var (queueName, implementationType) in targetServices)
        {
            cfg.ReceiveEndpoint(queueName, endpoint =>
            {
                // Register the consumer for this endpoint using the DI context
                endpoint.Consumer(implementationType, _ => context.GetService(implementationType));
            });
        }
    }

    /// <summary>
    /// Scans all loaded assemblies for types decorated with <see cref="ReceiveEndpointAttribute"/> and yields their queue name and implementation type.
    /// </summary>
    /// <returns>
    /// An enumerable of tuples containing the queue name and implementation type.
    /// </returns>
    private static IEnumerable<(string QueueName, Type ImplementationType)> GetServices()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                // Check for ReceiveEndpointAttribute and yield if present
                var receiveEndpoint = type.GetCustomAttribute<ReceiveEndpointAttribute>();
                if (receiveEndpoint is not null)
                {
                    yield return (receiveEndpoint.QueueName, type);
                }
            }
        }
    }
}