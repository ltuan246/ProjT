// RedisConnection provides an implementation of the IRedisConnection interface for interacting with a Redis cache.
namespace KISS.Caching.Redis;

/// <summary>
/// Provides a sealed implementation of <see cref="IRedisConnection"/> using an <see cref="IConnectionMultiplexer"/> to interact with Redis.
/// Supports asynchronous get and set operations for string key-value pairs.
/// </summary>
/// <param name="Redis">The Redis connection multiplexer instance.</param>
public sealed record RedisConnection(IConnectionMultiplexer Redis) : IRedisConnection
{
    /// <summary>
    /// Asynchronously retrieves the value associated with the specified key from the Redis database.
    /// </summary>
    /// <param name="key">The key whose value should be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value as a string if the key exists; otherwise, <c>null</c>.</returns>
    public async Task<string?> GetAsync(string key)
    {
        var db = Redis.GetDatabase();
        return await db.StringGetAsync(key);
    }

    /// <summary>
    /// Asynchronously sets the value for the specified key in the Redis database, with optional expiration.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to associate with the key.</param>
    /// <param name="expiry">The optional expiration time for the key-value pair.</param>
    /// <returns>A task representing the asynchronous set operation.</returns>
    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        var db = Redis.GetDatabase();
        await db.StringSetAsync(key, value, expiry);
    }
}