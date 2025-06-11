namespace KISS.Caching.Redis;

/// <summary>
/// Represents a connection to a Redis cache, providing basic operations for string values and access to low-level Redis interfaces.
/// </summary>
public interface IRedisConnection : ICacheService
{
    /// <summary>
    /// Gets the low-level Redis database interface for advanced operations.
    /// </summary>
    IDatabase Db { get; }

    /// <summary>
    /// Gets the Redis pub/sub subscriber interface for messaging operations.
    /// </summary>
    ISubscriber Subscriber { get; }
}