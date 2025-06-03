namespace KISS.Caching.Redis;

/// <summary>
/// Provides extension methods for registering and configuring Redis services in the dependency injection container.
/// </summary>
public static class ServiceRegistrationExtensions
{
    /// <summary>
    /// Registers and configures a Redis connection using settings from the specified configuration section.
    /// Binds <see cref="RedisConfigurationOption"/> to the configuration, validates it, and adds a singleton <see cref="IConnectionMultiplexer"/>.
    /// </summary>
    /// <param name="services">The service collection to register Redis services into.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="sectionName">The configuration section name containing Redis settings. Defaults to "SmtpClientOptions".</param>
    public static void UseRedis(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "SmtpClientOptions")
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);

        ArgumentNullException.ThrowIfNull(section);

        // Bind Redis configuration options and validate on startup
        services
            .AddOptions<RedisConfigurationOption>()
            .Bind(section)
            .ValidateOnStart();

        // Register the Redis connection multiplexer as a singleton
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<RedisConfigurationOption>>().Value;

            if (config.EndPoints == null || config.EndPoints.Count == 0)
            {
                throw new InvalidOperationException("Redis Endpoints are required.");
            }

            // Map configuration options to StackExchange.Redis ConfigurationOptions
            var options = new ConfigurationOptions
            {
                AbortOnConnectFail = config.AbortOnConnectFail,
                AllowAdmin = config.AllowAdmin,
                ClientName = config.ClientName,
                ConnectRetry = config.ConnectRetry,
                ConnectTimeout = config.ConnectTimeout,
                DefaultDatabase = config.DefaultDatabase,
                KeepAlive = config.KeepAlive,
                Password = config.Password,
                Proxy = (Proxy)Enum.Parse(typeof(Proxy), config.Proxy),
                ResolveDns = config.ResolveDns,
                ServiceName = config.ServiceName,
                Ssl = config.Ssl,
                SslHost = config.SslHost,
                SyncTimeout = config.SyncTimeout,
                TieBreaker = config.TieBreaker,
                User = config.User,
            };

            // Add all configured endpoints
            foreach (var endpoint in config.EndPoints)
            {
                options.EndPoints.Add(endpoint);
            }

            // Create and return the connection multiplexer
            return ConnectionMultiplexer.Connect(options);
        });

        services.AddSingleton<IRedisConnection, RedisConnection>();
    }
}