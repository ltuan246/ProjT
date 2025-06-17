namespace KISS.Caching.Strategies.Redis;

/// <summary>
/// Defines a contract for obtaining a Redis <see cref="ConnectionMultiplexer"/> instance.
/// </summary>
public interface IRedisConnection
{
    /// <summary>
    /// Gets the Redis connection multiplexer used for interacting with the Redis server.
    /// </summary>
    ConnectionMultiplexer Connection { get; }
}