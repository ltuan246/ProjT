namespace KISS.Caching;

/// <summary>
/// Provides extension methods for registering cache stores and strategies in the dependency injection container.
/// </summary>
public static class ServiceRegistrationExtensions
{
    /// <summary>
    /// Registers a cache store implementation and its associated caching strategies in the dependency injection container.
    /// </summary>
    /// <typeparam name="T">The cache store implementation type (must implement <see cref="ICacheStorage"/>).</typeparam>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="cacheStrategies">The caching strategies to register for this store.</param>
    public static void UseCacheStore<T>(
        this IServiceCollection services,
        params CacheStrategies[] cacheStrategies)
        where T : ICacheStorage
    {
        var cacheStore = typeof(T).GetCustomAttribute<CacheStoreAttribute>()!.CacheStore;

        switch (cacheStore)
        {
            case CacheStores.InMemory:
                services.AddMemoryCache();
                // Register the cache store implementation as a keyed singleton.
                services.AddKeyedSingleton<ICacheStorage>(
                    cacheStore,
                    (sp, key) => new InMemoryCacheStorage(sp.GetRequiredService<IMemoryCache>()));
                break;
            case CacheStores.Distributed:
                services.AddDistributedMemoryCache();
                // Register the cache store implementation as a keyed singleton.
                services.AddKeyedSingleton<ICacheStorage>(
                    cacheStore,
                    (sp, key) => new DistributedCacheStorage(sp.GetRequiredService<IDistributedCache>()));
                break;

            default:
                break;
        }

        if (cacheStrategies?.Length > 0)
        {
            foreach (var strategy in cacheStrategies)
            {
                switch (strategy)
                {
                    case CacheStrategies.CacheAside:
                        // Register the CacheAside strategy as a keyed singleton.
                        services.AddKeyedSingleton<ICacheStrategy>((cacheStore, strategy), (sp, key) =>
                            new CacheAsideStrategy(sp.GetRequiredKeyedService<ICacheStorage>(cacheStore), sp.GetRequiredService<IDataStorage>()));
                        break;
                    case CacheStrategies.WriteThrough:
                        // Register the WriteThrough strategy as a keyed singleton.
                        services.AddKeyedSingleton<ICacheStrategy>((cacheStore, strategy), (sp, key) =>
                            new WriteThroughStrategy(sp.GetRequiredKeyedService<ICacheStorage>(cacheStore), sp.GetRequiredService<IDataStorage>()));
                        break;
                    case CacheStrategies.ReadThrough:
                        // Register the ReadThrough strategy as a keyed singleton.
                        services.AddKeyedSingleton<ICacheStrategy>((cacheStore, strategy), (sp, key) =>
                            new ReadThroughStrategy(sp.GetRequiredKeyedService<ICacheStorage>(cacheStore), sp.GetRequiredService<IDataStorage>()));
                        break;
                    case CacheStrategies.WriteBack:
                        // Register the WriteBack strategy as a keyed singleton.
                        services.AddKeyedSingleton<ICacheStrategy>((cacheStore, strategy), (sp, key) =>
                            new WriteBackStrategy(sp.GetRequiredKeyedService<ICacheStorage>(cacheStore), sp.GetRequiredService<IDataStorage>()));
                        break;
                    case CacheStrategies.WriteAround:
                        // Register the WriteAround strategy as a keyed singleton.
                        services.AddKeyedSingleton<ICacheStrategy>((cacheStore, strategy), (sp, key) =>
                            new WriteAroundStrategy(sp.GetRequiredKeyedService<ICacheStorage>(cacheStore), sp.GetRequiredService<IDataStorage>()));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}