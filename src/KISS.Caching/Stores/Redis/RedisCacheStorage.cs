namespace KISS.Caching.Stores.Redis;

/// <summary>
///     Provides a Redis-backed implementation of <see cref="ICacheStorage" /> using <see cref="IConnectionMultiplexer" />
///     and MessagePack serialization.
/// </summary>
public sealed record RedisCacheStorage(IConnectionMultiplexer Redis) : ICacheStorage
{
    /// <summary>
    ///     Gets the Redis database instance for cache operations.
    /// </summary>
    public IDatabase Db { get; } = Redis.GetDatabase();

    /// <summary>
    ///     Gets the Redis subscriber instance for pub/sub operations.
    /// </summary>
    public ISubscriber Subscriber { get; } = Redis.GetSubscriber();

    /// <summary>
    ///     Asynchronously retrieves a cached value by its key from Redis.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The unique key identifying the cached item.</param>
    /// <returns>
    ///     A <see cref="CacheResult{T}" /> containing the cached value, or a null result if not found or deserialization
    ///     fails.
    /// </returns>
    public async Task<CacheResult<T>> GetAsync<T>(string key)
    {
        var cachedData = await Db.StringGetAsync(key);
        if (cachedData.IsNullOrEmpty)
        {
            return CacheResult<T>.Null();
        }

        try
        {
            var value = MessagePackSerializer.Deserialize<T>(cachedData);
            return value == null ? CacheResult<T>.Null() : CacheResult<T>.Success(value);
        }
        catch (MessagePackSerializationException)
        {
            return CacheResult<T>.Null();
        }
    }

    /// <summary>
    ///     Asynchronously stores a value in Redis with the specified cache options.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The unique key to associate with the cached item.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="options">Cache expiration and behavior options.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SetAsync<T>(string key, T value, CacheMechanismOptions? options)
    {
        var serializedValue = MessagePackSerializer.Serialize(value);
        await Db.StringSetAsync(key, serializedValue, options?.SlidingExpiration);
    }

    /// <summary>
    ///     Asynchronously removes a cached item from Redis by its key.
    /// </summary>
    /// <param name="key">The unique key identifying the cached item to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RemoveAsync(string key)
        => await Db.KeyDeleteAsync(key);
}
