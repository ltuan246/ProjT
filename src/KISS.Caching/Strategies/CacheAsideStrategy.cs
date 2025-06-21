namespace KISS.Caching.Strategies;

/// <summary>
///     Implements the cache-aside pattern: checks the cache first, and on a miss, allows the application to provide custom
///     data retrieval to fetch and populate the cache.
/// </summary>
/// <param name="CacheStorage">The cache storage mechanism.</param>
/// <param name="DataStorage">The underlying data storage mechanism.</param>
[CacheStrategy(CacheStrategy.CacheAside)]
public sealed record CacheAsideStrategy(ICacheStorage CacheStorage, IDataStorage DataStorage) : ICacheStrategy
{
    /// <inheritdoc />
    public async Task<CacheResult<T>> GetOrSetAsync<T>(string key, T value, CacheMechanismOptions? options)
    {
        var cachedValue = await CacheStorage.GetAsync<T>(key);
        if (cachedValue.HasValue)
        {
            return cachedValue;
        }

        await CacheStorage.SetAsync(key, value, options);
        return CacheResult<T>.Success(value);
    }

    /// <inheritdoc />
    public async Task UpdateAsync<T>(string key, T value, CacheMechanismOptions? options)
        // Updates the data storage, invalidates the cache key.
        => await Task.WhenAll(
            DataStorage.UpdateAsync(key, value),
            CacheStorage.RemoveAsync(key));
}
