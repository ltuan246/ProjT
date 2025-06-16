namespace KISS.Caching.Strategies;

/// <summary>
///     Provides methods for basic cache operations such as retrieving and updating cached items.
/// </summary>
public interface IDataStorage
{
    /// <summary>
    ///     Asynchronously retrieves a cached item by its key.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>A task representing the asynchronous strategy, with the cached value as its result.</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    ///     Asynchronously updates or adds a cached item with the specified key and value.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to store in the cache.</param>
    /// <returns>A task representing the asynchronous strategy.</returns>
    Task UpdateAsync<T>(string key, T value);
}
