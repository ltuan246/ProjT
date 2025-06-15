namespace KISS.Caching.Strategies;

/// <summary>
///     Defines a contract for cache strategies in KISS.Caching.
///     <para>
///         Implement this interface to provide custom caching logic, such as in-memory, distributed, or hybrid caching.
///     </para>
/// </summary>
public interface ICacheStorage
{
    /// <summary>
    ///     Asynchronously retrieves a cached value by its key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The unique key identifying the cached item.</param>
    /// <returns>The cached value if found; otherwise, the default value for <typeparamref name="T" />.</returns>
    Task<CacheResult<T>> GetAsync<T>(string key);

    /// <summary>
    ///     Asynchronously stores a value in the cache with the specified options.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The unique key to associate with the cached item.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="options">Cache expiration and behavior options.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, CacheMechanismOptions? options);

    /// <summary>
    ///     Asynchronously removes a cached item by its key.
    /// </summary>
    /// <param name="key">The unique key identifying the cached item to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(string key);
}

// public class CacheStorage
// {
//     private readonly ICacheOperation _cacheOperation;
//     public CacheStorage(ICacheOperation cacheOperation)
//     {
//         _cacheOperation = cacheOperation ?? throw new ArgumentNullException(nameof(cacheOperation));
//     }
//     public Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, CacheMechanismOptions options)
//     {
//         return _cacheOperation.GetOrSetAsync(key, factory, options);
//     }
//     public Task UpdateAsync<T>(string key, T value, CacheMechanismOptions options)
//     {
//         return _cacheOperation.UpdateAsync(key, value, options);
//     }
// }
