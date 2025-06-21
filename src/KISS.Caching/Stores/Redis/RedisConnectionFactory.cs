namespace KISS.Caching.Stores.Redis;

/// <summary>
/// Provides a factory for creating and configuring Redis <see cref="ConnectionMultiplexer"/> instances from application configuration.
/// </summary>
public static class RedisConnectionFactory
{
    /// <summary>
    /// Creates and configures a <see cref="IConnectionMultiplexer"/> instance using the specified Redis configuration options.
    /// </summary>
    /// <param name="options">The Redis configuration options.</param>
    /// <returns>A configured <see cref="IConnectionMultiplexer"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no Redis endpoints are configured.</exception>
    public static IConnectionMultiplexer Create(IOptions<RedisConfigurationOption> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var config = options.Value;

        if (config.EndPoints == null || config.EndPoints.Count == 0)
        {
            throw new InvalidOperationException("Redis Endpoints are required.");
        }

        // Map configuration options to StackExchange.Redis ConfigurationOptions
        var redisOptions = new ConfigurationOptions
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
            User = config.User
        };

        // Add all configured endpoints
        foreach (var endpoint in config.EndPoints)
        {
            redisOptions.EndPoints.Add(endpoint);
        }

        // Create and return the connection multiplexer
        return ConnectionMultiplexer.Connect(redisOptions);
    }
}
