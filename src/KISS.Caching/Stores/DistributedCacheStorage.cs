namespace KISS.Caching.Stores;

/// <summary>
///     Provides a distributed cache strategy using <see cref="IDistributedCache" /> for storing and retrieving cached
///     items in a distributed environment.
/// </summary>
/// <param name="Cache">The distributed cache instance.</param>
[CacheStore(CacheStore.Distributed)]
public sealed record DistributedCacheStorage(IDistributedCache Cache) : ICacheStorage
{
    /// <inheritdoc />
    public async Task<CacheResult<T>> GetAsync<T>(string key)
    {
        var cachedData = await Cache.GetAsync(key);
        if (cachedData == null)
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

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, CacheMechanismOptions? options)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = options?.SlidingExpiration,
            AbsoluteExpirationRelativeToNow = options?.AbsoluteExpiration
        };

        var serializedData = MessagePackSerializer.Serialize(value);
        await Cache.SetAsync(key, serializedData, cacheOptions);
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key) => Cache.RemoveAsync(key);
}
