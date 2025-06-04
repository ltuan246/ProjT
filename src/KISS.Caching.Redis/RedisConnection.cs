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
    public async Task<string?> GetAsync(string key)
        => await Db.StringGetAsync(key);

    /// <inheritdoc />
    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        => await Db.StringSetAsync(key, value, expiry);

    /// <inheritdoc />
    public async Task DeleteAsync(string key)
        => await Db.KeyDeleteAsync(key);
}