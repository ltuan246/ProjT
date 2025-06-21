namespace KISS.Caching.Strategies;

/// <summary>
///     Implements the write-back caching pattern, writing to cache immediately and deferring data storage updates.
/// </summary>
/// <param name="CacheStorage">The cache storage mechanism.</param>
/// <param name="DataStorage">The underlying data storage mechanism.</param>
[CacheStrategy(CacheStrategy.WriteBack)]
public sealed record WriteBackStrategy(ICacheStorage CacheStorage, IDataStorage DataStorage)
    : ICacheStrategy, IDisposable
{
    /// <summary>
    ///     Queue of pending asynchronous data storage update operations.
    /// </summary>
    private BackgroundTaskProcessor Queue { get; } = new();

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
    {
        // Write to cache immediately
        await CacheStorage.SetAsync(key, value, options);

        // Queue data source update for asynchronous execution
        Queue.Enqueue(() => DataStorage.UpdateAsync(key, value));
    }

    /// <inheritdoc />
    public void Dispose() => Queue.Dispose();
}
