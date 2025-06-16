namespace KISS.Caching.Tests;

public sealed record CacheStorage(ICacheStrategy Strategy)
{
    public Task<CacheResult<T>> GetOrSetAsync<T>(string key, T value, CacheMechanismOptions? options)
        => Strategy.GetOrSetAsync(key, value, options);

    public Task UpdateAsync<T>(string key, T value, CacheMechanismOptions? options)
        => Strategy.UpdateAsync(key, value, options);
}