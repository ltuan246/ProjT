namespace KISS.Caching.Redis;

/// <summary>
/// Provides a sealed implementation of <see cref="IRedisConnection"/> using an <see cref="IConnectionMultiplexer"/> to interact with Redis.
/// Supports asynchronous get and set operations for string key-value pairs.
/// </summary>
/// <param name="Redis">The Redis connection multiplexer instance.</param>
public sealed record RedisConnection(IConnectionMultiplexer Redis) : IRedisConnection
{
    /// <inheritdoc />
    public IDatabase Db { get; } = Redis.GetDatabase();

    /// <inheritdoc />
    public ISubscriber Subscriber { get; } = Redis.GetSubscriber();

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key)
    {
        var cachedValue = await Db.StringGetAsync(key);
        return cachedValue.IsNullOrEmpty ? default : MessagePackSerializer.Deserialize<T>(cachedValue!);
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var serializedValue = MessagePackSerializer.Serialize(value);
        await Db.StringSetAsync(key, serializedValue, expiry);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string key)
        => await Db.KeyDeleteAsync(key);
}