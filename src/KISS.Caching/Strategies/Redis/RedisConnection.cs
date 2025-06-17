namespace KISS.Caching.Strategies.Redis;

/// <summary>
/// Defines a contract for obtaining a Redis <see cref="ConnectionMultiplexer"/> instance.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RedisConnection"/> class with the specified connection string.
/// </remarks>
/// <param name="connectionString">The Redis connection string.</param>
public sealed class RedisConnection(string connectionString) : IRedisConnection
{
    private Lazy<ConnectionMultiplexer> LazyConnection { get; } = new(() => ConnectionMultiplexer.Connect(connectionString));

    /// <summary>
    /// Gets the Redis connection multiplexer used for interacting with the Redis server.
    /// </summary>
    public ConnectionMultiplexer Connection => LazyConnection.Value;
}