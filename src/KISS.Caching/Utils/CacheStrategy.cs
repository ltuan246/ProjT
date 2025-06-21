namespace KISS.Caching.Utils;

/// <summary>
///     Enumerates the caching strategies supported by KISS.Caching, describing how cache and data storage interact.
/// </summary>
public enum CacheStrategy
{
    /// <summary>
    ///     Cache-Aside: The application loads data into the cache on demand and is responsible for cache population and
    ///     invalidation.
    /// </summary>
    CacheAside,

    /// <summary>
    ///     Write-Through: Data is written to both the cache and the underlying data storage synchronously.
    /// </summary>
    WriteThrough,

    /// <summary>
    ///     Read-Through: Data is fetched from the cache first; on a miss, it is loaded from data storage and then cached.
    /// </summary>
    ReadThrough,

    /// <summary>
    ///     Write-Behind (Write-Back): Data is written to the cache immediately and asynchronously persisted to data storage.
    /// </summary>
    WriteBack,

    /// <summary>
    ///     Write-Around: Data is written directly to data storage, bypassing the cache; the cache is not updated or have keys
    ///     removed.
    /// </summary>
    WriteAround
}
