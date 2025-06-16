namespace KISS.Caching;

/// <summary>
/// Provides extension methods for registering cache stores and strategies in the dependency injection container.
/// </summary>
public static class ServiceRegistrationExtensions
{
    /// <summary>
    /// Generates a cache key string based on the specified cache store and strategy.
    /// </summary>
    /// <param name="cacheStores">The cache store type (e.g., InMemory, Distributed).</param>
    /// <param name="cacheStrategies">The cache strategy (e.g., CacheAside, WriteThrough).</param>
    /// <returns>A string representing the composite cache key.</returns>
    public static string GetCacheKey(this CacheStores cacheStores, CacheStrategies cacheStrategies)
        => $"{cacheStores}_{cacheStrategies}";

    /// <summary>
    /// Registers a cache store implementation and its associated caching strategies in the dependency injection container.
    /// </summary>
    /// <typeparam name="T">The cache store implementation type (must implement <see cref="ICacheStorage"/>).</typeparam>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="cacheStrategies">The caching strategies to register for this store.</param>
    public static void UseCacheStore<T>(
        this IServiceCollection services,
        params CacheStrategies[] cacheStrategies)
        where T : class, ICacheStorage
    {
        var cacheStore = typeof(T).GetCustomAttribute<CacheStoreAttribute>()!.CacheStore;

        // Register the cache store implementation as a keyed singleton.
        services.AddKeyedSingleton<ICacheStorage>(
            cacheStore,
            (sp, key) => new InMemoryCacheStorage(sp.GetRequiredService<IMemoryCache>()));

        if (cacheStrategies?.Length > 0)
        {
            foreach (var strategy in cacheStrategies)
            {
                switch (strategy)
                {
                    case CacheStrategies.CacheAside:
                        // Register the CacheAside strategy as a keyed singleton.
                        services.AddKeyedSingleton<ICacheStrategy>($"{cacheStore}_{strategy}", (sp, key) =>
                            new CacheAsideStrategy(sp.GetRequiredKeyedService<ICacheStorage>(cacheStore), sp.GetRequiredService<IDataStorage>()));
                        break;
                    case CacheStrategies.WriteThrough:
                        // Register the WriteThrough strategy as a keyed singleton.
                        services.AddKeyedSingleton<ICacheStrategy>($"{cacheStore}_{strategy}", (sp, key) =>
                            new WriteThroughStrategy(sp.GetRequiredKeyedService<ICacheStorage>(cacheStore), sp.GetRequiredService<IDataStorage>()));
                        break;
                    case CacheStrategies.ReadThrough:
                        // Register the ReadThrough strategy as a keyed singleton.
                        services.AddKeyedSingleton<ICacheStrategy>($"{cacheStore}_{strategy}", (sp, key) =>
                            new ReadThroughStrategy(sp.GetRequiredKeyedService<ICacheStorage>(cacheStore), sp.GetRequiredService<IDataStorage>()));
                        break;
                    case CacheStrategies.WriteBack:
                        // Register the WriteBack strategy as a keyed singleton.
                        services.AddKeyedSingleton<ICacheStrategy>($"{cacheStore}_{strategy}", (sp, key) =>
                            new WriteBackStrategy(sp.GetRequiredKeyedService<ICacheStorage>(cacheStore), sp.GetRequiredService<IDataStorage>()));
                        break;
                    case CacheStrategies.WriteAround:
                        // Register the WriteAround strategy as a keyed singleton.
                        services.AddKeyedSingleton<ICacheStrategy>($"{cacheStore}_{strategy}", (sp, key) =>
                            new WriteAroundStrategy(sp.GetRequiredKeyedService<ICacheStorage>(cacheStore), sp.GetRequiredService<IDataStorage>()));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}