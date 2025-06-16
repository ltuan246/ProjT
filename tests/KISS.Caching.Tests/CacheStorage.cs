namespace KISS.Caching.Tests;

public sealed record CacheStorage(ICacheOperation Operation)
{
    public Task<CacheResult<T>> GetOrSetAsync<T>(string key, T value, CacheMechanismOptions? options)
        => Operation.GetOrSetAsync(key, value, options);

    public Task UpdateAsync<T>(string key, T value, CacheMechanismOptions? options)
        => Operation.UpdateAsync(key, value, options);
}