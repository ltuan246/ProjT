namespace KISS.Caching.Strategies;

/// <summary>
///     Provides an in-memory cache strategy using <see cref="IMemoryCache" /> for storing and retrieving cached items.
/// </summary>
/// <param name="Cache">The in-memory cache instance.</param>
public sealed record InMemoryCacheStrategy(IMemoryCache Cache) : ICacheStorage
{
    /// <inheritdoc />
    public Task<CacheResult<T>> GetAsync<T>(string key)
        => Cache.TryGetValue(key, out T? value)
            ? Task.FromResult(value == null ? CacheResult<T>.Null() : CacheResult<T>.Success(value))
            : Task.FromResult(CacheResult<T>.Null());

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value, CacheMechanismOptions? options)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = options?.SlidingExpiration,
            AbsoluteExpirationRelativeToNow = options?.AbsoluteExpiration
        };

        Cache.Set(key, value, cacheOptions);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key)
    {
        Cache.Remove(key);
        return Task.CompletedTask;
    }
}
