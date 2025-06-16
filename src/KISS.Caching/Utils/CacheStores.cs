namespace KISS.Caching.Utils;

/// <summary>
/// Specifies the available cache storage types supported by KISS.Caching.
/// </summary>
public enum CacheStores
{
    /// <summary>
    /// Represents in-memory cache storage (local to the application instance).
    /// </summary>
    InMemory,

    /// <summary>
    /// Represents distributed cache storage (shared across multiple application instances).
    /// </summary>
    Distributed
}
