namespace KISS.Caching.Operations;

/// <summary>
///     Implements the read-through caching pattern, automatically loading data from the data source on cache miss and
///     updating the cache.
/// </summary>
/// <param name="CacheStorage">The cache storage mechanism.</param>
/// <param name="DataStorage">The underlying data storage mechanism.</param>
public sealed record ReadThroughOperation(ICacheStorage CacheStorage, IDataStorage DataStorage) : ICacheOperation
{
    /// <inheritdoc />
    public async Task<CacheResult<T>> GetOrSetAsync<T>(string key, T value, CacheMechanismOptions? options)
    {
        var cachedResult = await CacheStorage.GetAsync<T>(key);
        if (cachedResult.HasValue)
        {
            return cachedResult;
        }

        var valueResult = await DataStorage.GetAsync<T>(key);
        if (valueResult is not null)
        {
            await CacheStorage.SetAsync(key, valueResult, options);
            return CacheResult<T>.Success(valueResult);
        }

        return CacheResult<T>.Null();
    }

    /// <inheritdoc />
    public async Task UpdateAsync<T>(string key, T value, CacheMechanismOptions? options)
        // Updates the data storage, invalidates the cache key.
        => await Task.WhenAll(
            DataStorage.UpdateAsync(key, value),
            CacheStorage.RemoveAsync(key));
}
