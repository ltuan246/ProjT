namespace KISS.Caching.Utils;

/// <summary>
///     Specifies the available cache storage types supported by KISS.Caching.
/// </summary>
public enum CacheStore
{
    /// <summary>
    ///     Represents in-memory cache storage (local to the application instance).
    /// </summary>
    InMemory,

    /// <summary>
    ///     Represents distributed cache storage (shared across multiple application instances).
    /// </summary>
    Distributed,

    /// <summary>
    ///     Represents Redis cache storage.
    /// </summary>
    Redis
}
