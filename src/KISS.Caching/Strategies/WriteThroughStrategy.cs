namespace KISS.Caching.Strategies;

/// <summary>
///     Implements the write-through caching pattern, ensuring data is written to both cache and data storage.
/// </summary>
/// <param name="CacheStorage">The cache storage mechanism.</param>
/// <param name="DataStorage">The underlying data storage mechanism.</param>
public sealed record WriteThroughStrategy(ICacheStorage CacheStorage, IDataStorage DataStorage) : ICacheStrategy
{
    /// <inheritdoc />
    public async Task<CacheResult<T>> GetOrSetAsync<T>(string key, T value, CacheMechanismOptions? options)
    {
        var cachedResult = await CacheStorage.GetAsync<T>(key);
        if (cachedResult.HasValue)
        {
            return cachedResult;
        }

        await CacheStorage.SetAsync(key, value, options);
        return CacheResult<T>.Success(value);
    }

    /// <inheritdoc />
    public async Task UpdateAsync<T>(string key, T value, CacheMechanismOptions? options)
        // Update both the data storage and cache
        // This ensures that the cache is always in sync with the data storage.
        => await Task.WhenAll(
            DataStorage.UpdateAsync(key, value),
            CacheStorage.SetAsync(key, value, options));
}
