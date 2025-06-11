namespace KISS.Caching.Redis;

/// <summary>
/// Defines a generic cache service interface for interacting with a Redis cache, supporting basic CRUD operations for any serializable type.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Asynchronously retrieves a value of type <typeparamref name="T"/> associated with the specified key from the cache.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key whose value should be retrieved.</param>
    /// <returns>The value of type <typeparamref name="T"/> if the key exists; otherwise, the default value for <typeparamref name="T"/>.</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Asynchronously sets a value of type <typeparamref name="T"/> for the specified key in the cache, with an optional expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to assign to the key.</param>
    /// <param name="expiry">The optional expiration time for the key. If null, the key will not expire.</param>
    /// <returns>A task representing the asynchronous set operation.</returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

    /// <summary>
    /// Asynchronously deletes the specified key and its value from the cache.
    /// </summary>
    /// <param name="key">The key to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteAsync(string key);
}