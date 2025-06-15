namespace KISS.Caching.Operations;

/// <summary>
///     Defines methods for advanced cache operations, such as retrieving, setting, and updating cached values.
/// </summary>
public interface ICacheOperation
{
    /// <summary>
    ///     Asynchronously retrieves a cached value by its key, or sets and returns a new value if it does not exist.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The unique key identifying the cached item.</param>
    /// <param name="value">The value to set if the item does not exist in the cache.</param>
    /// <param name="options">Options for cache expiration and behavior.</param>
    /// <returns>A <see cref="CacheResult{T}" /> containing the cached or newly set value.</returns>
    Task<CacheResult<T>> GetOrSetAsync<T>(string key, T value, CacheMechanismOptions? options);

    /// <summary>
    ///     Asynchronously updates the value of an existing cached item.
    /// </summary>
    /// <typeparam name="T">The type of the value to update.</typeparam>
    /// <param name="key">The unique key identifying the cached item to update.</param>
    /// <param name="value">The new value to store in the cache.</param>
    /// <param name="options">Options for cache expiration and behavior.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    Task UpdateAsync<T>(string key, T value, CacheMechanismOptions? options);
}
