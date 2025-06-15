namespace KISS.Caching.Operations;

/// <summary>
///     Implements the write-around caching pattern by writing directly to the data source and bypassing the cache.
/// </summary>
/// <param name="CacheStorage">The cache storage mechanism.</param>
/// <param name="DataStorage">The underlying data storage mechanism.</param>
public sealed record WriteAroundOperation(ICacheStorage CacheStorage, IDataStorage DataStorage) : ICacheOperation
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
        // In the write-around pattern, only the data storage is updated; the cache is not updated on writes.
        => await DataStorage.UpdateAsync(key, value);
}
