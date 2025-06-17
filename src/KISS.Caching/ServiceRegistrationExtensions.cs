namespace KISS.Caching;

/// <summary>
/// Provides extension methods for registering cache stores and strategies, including Redis, in the dependency injection container.
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
        params CacheStrategy[] cacheStrategies)
        where T : ICacheStorage
    {
        var cacheStore = typeof(T).GetCustomAttribute<CacheStoreAttribute>()!.CacheStore;

        switch (cacheStore)
        {
            case CacheStore.InMemory:
                // Register in-memory cache and its implementation as a keyed singleton.
                services.AddMemoryCache();
                services.AddKeyedSingleton<ICacheStorage>(
                    cacheStore,
                    (sp, key) => new InMemoryCacheStorage(sp.GetRequiredService<IMemoryCache>()));
                break;
            case CacheStore.Distributed:
                // Register distributed memory cache and its implementation as a keyed singleton.
                services.AddDistributedMemoryCache();
                services.AddKeyedSingleton<ICacheStorage>(
                    cacheStore,
                    (sp, key) => new DistributedCacheStorage(sp.GetRequiredService<IDistributedCache>()));
                break;
            case CacheStore.Redis:
                // Register Redis cache store as a keyed singleton (configuration handled separately).
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
                services.UseCacheStrategy(cacheStore, strategy);
            }
        }
    }

    /// <summary>
    /// Registers Redis cache configuration, connection, and strategies in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="sectionName">The configuration section name for Redis settings (default: "Redis").</param>
    /// <param name="cacheStrategies">The caching strategies to register for Redis.</param>
    public static void UseRedis(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Redis",
        params CacheStrategy[] cacheStrategies)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);

        ArgumentNullException.ThrowIfNull(section);

        // Bind Redis configuration options and validate on startup
        services
            .AddOptions<RedisConfigurationOption>()
            .Bind(section)
            .ValidateOnStart();

        // Register the Redis connection multiplexer as a singleton
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RedisConfigurationOption>>();
            return RedisConnectionFactory.Create(options);
        });

        // Register the Redis connection abstraction
        services.AddSingleton<IRedisConnection, RedisConnection>();

        // Register the distributed cache using StackExchange.Redis
        services.AddStackExchangeRedisCache(options =>
            {
                options.ConnectionMultiplexerFactory = () => Task.FromResult(services.BuildServiceProvider().GetRequiredService<IConnectionMultiplexer>());
                options.InstanceName = "Redis_";
            });

        // Register the Redis cache store as a keyed singleton.
        services.AddKeyedSingleton<ICacheStorage>(
            CacheStore.Redis,
            (sp, key) => new DistributedCacheStorage(sp.GetRequiredService<IDistributedCache>()));

        if (cacheStrategies?.Length > 0)
        {
            foreach (var strategy in cacheStrategies)
            {
                services.UseCacheStrategy(CacheStore.Redis, strategy);
            }
        }
    }

    private static void UseCacheStrategy(
        this IServiceCollection services,
        CacheStore store,
        CacheStrategy strategy)
    {
        switch (strategy)
        {
            case CacheStrategy.CacheAside:
                services.AddKeyedSingleton<ICacheStrategy>((store, strategy), (sp, key) =>
                    new CacheAsideStrategy(sp.GetRequiredKeyedService<ICacheStorage>(store), sp.GetRequiredService<IDataStorage>()));
                break;
            case CacheStrategy.WriteThrough:
                services.AddKeyedSingleton<ICacheStrategy>((store, strategy), (sp, key) =>
                    new WriteThroughStrategy(sp.GetRequiredKeyedService<ICacheStorage>(store), sp.GetRequiredService<IDataStorage>()));
                break;
            case CacheStrategy.ReadThrough:
                services.AddKeyedSingleton<ICacheStrategy>((store, strategy), (sp, key) =>
                    new ReadThroughStrategy(sp.GetRequiredKeyedService<ICacheStorage>(store), sp.GetRequiredService<IDataStorage>()));
                break;
            case CacheStrategy.WriteBack:
                services.AddKeyedSingleton<ICacheStrategy>((store, strategy), (sp, key) =>
                    new WriteBackStrategy(sp.GetRequiredKeyedService<ICacheStorage>(store), sp.GetRequiredService<IDataStorage>()));
                break;
            case CacheStrategy.WriteAround:
                services.AddKeyedSingleton<ICacheStrategy>((store, strategy), (sp, key) =>
                    new WriteAroundStrategy(sp.GetRequiredKeyedService<ICacheStorage>(store), sp.GetRequiredService<IDataStorage>()));
                break;
            default:
                break;
        }
    }
}
