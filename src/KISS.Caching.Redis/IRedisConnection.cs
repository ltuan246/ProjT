namespace KISS.Caching.Redis;

/// <summary>
/// Represents a connection to a Redis cache, providing basic operations for string values and access to low-level Redis interfaces.
/// </summary>
public interface IRedisConnection
{
    /// <summary>
    /// Gets the low-level Redis database interface for advanced operations.
    /// </summary>
    IDatabase Db { get; }

    /// <summary>
    /// Gets the Redis pub/sub subscriber interface for messaging operations.
    /// </summary>
    ISubscriber Subscriber { get; }

    /// <summary>
    /// Asynchronously retrieves the value associated with the specified key from Redis.
    /// </summary>
    /// <param name="key">The key whose value should be retrieved.</param>
    /// <returns>The value as a string if the key exists; otherwise, null.</returns>
    Task<string?> GetAsync(string key);

    /// <summary>
    /// Asynchronously sets the value for the specified key in Redis, with an optional expiration time.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to assign to the key.</param>
    /// <param name="expiry">The optional expiration time for the key. If null, the key will not expire.</param>
    /// <returns>A task representing the asynchronous set operation.</returns>
    Task SetAsync(string key, string value, TimeSpan? expiry = null);

    /// <summary>
    /// Asynchronously deletes the specified key and its value from Redis.
    /// </summary>
    /// <param name="key">The key to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteAsync(string key);
}