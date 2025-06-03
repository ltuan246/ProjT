// IRedisConnection defines the contract for interacting with a Redis cache.
// Provides asynchronous methods for getting and setting string values by key.
namespace KISS.Caching.Redis;

/// <summary>
/// Represents a connection to a Redis cache, providing basic get and set operations.
/// </summary>
public interface IRedisConnection
{
    /// <summary>
    /// Asynchronously retrieves the value associated with the specified key from Redis.
    /// </summary>
    /// <param name="key">The key whose value to retrieve.</param>
    /// <returns>The value as a string if found; otherwise, null.</returns>
    Task<string?> GetAsync(string key);

    /// <summary>
    /// Asynchronously sets the value for the specified key in Redis, with optional expiry.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="expiry">Optional expiration time for the key.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync(string key, string value, TimeSpan? expiry = null);
}