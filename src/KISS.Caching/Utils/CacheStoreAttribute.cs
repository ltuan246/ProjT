namespace KISS.Caching.Utils;

/// <summary>
///     Attribute for marking classes with a specific cache store type, enabling dynamic dependency resolution and lifetime
///     management.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CacheStoreAttribute(CacheStore cacheStore) : Attribute
{
    /// <summary>
    ///     Gets the cache store type associated with the attributed class.
    /// </summary>
    public CacheStore CacheStore { get; } = cacheStore;
}
