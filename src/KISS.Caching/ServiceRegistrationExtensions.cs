namespace KISS.Caching;

/// <summary>
///     Provides extension methods for registering cache stores and strategies, including Redis, in the dependency
///     injection container.
/// </summary>
public static class ServiceRegistrationExtensions
{
    /// <summary>
    ///     Registers the in-memory cache store and its associated caching strategies in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="cacheStrategies">The caching strategies to register for this store.</param>
    public static void UseMemoryCache(
        this IServiceCollection services,
        params CacheStrategy[] cacheStrategies)
    {
        // Register in-memory cache and its implementation as a keyed singleton.
        services.AddMemoryCache();
        services.AddKeyedSingleton<ICacheStorage>(
            CacheStore.InMemory,
            (sp, key) => new InMemoryCacheStorage(sp.GetRequiredService<IMemoryCache>()));

        if (cacheStrategies?.Length > 0)
        {
            foreach (var strategy in cacheStrategies)
            {
                services.UseCacheStrategy(CacheStore.InMemory, strategy);
            }
        }
    }

    /// <summary>
    ///     Registers the distributed memory cache store and its associated caching strategies in the dependency injection
    ///     container.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="cacheStrategies">The caching strategies to register for this store.</param>
    public static void UseDistributedMemoryCache(
        this IServiceCollection services,
        params CacheStrategy[] cacheStrategies)
    {
        services.AddDistributedMemoryCache();
        services.AddKeyedSingleton<ICacheStorage>(
            CacheStore.Distributed,
            (sp, key) => new DistributedCacheStorage(sp.GetRequiredService<IDistributedCache>()));

        if (cacheStrategies?.Length > 0)
        {
            foreach (var strategy in cacheStrategies)
            {
                services.UseCacheStrategy(CacheStore.Distributed, strategy);
            }
        }
    }

    /// <summary>
    ///     Registers Redis cache configuration, connection, and strategies in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <param name="sectionName">The configuration section name for Redis settings (default: "Redis").</param>
    /// <param name="instanceName">The instance name for the Redis cache (default: "Redis").</param>
    /// <param name="cacheStrategies">The caching strategies to register for Redis.</param>
    public static void UseRedis(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Redis",
        string instanceName = "Redis",
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

        // Register the Redis cache store as a keyed singleton.
        services.AddKeyedSingleton<ICacheStorage>(
            CacheStore.Redis,
            (sp, key) => new RedisCacheStorage(sp.GetRequiredService<IConnectionMultiplexer>()));

        // Register the distributed cache using StackExchange.Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.ConnectionMultiplexerFactory = () =>
                Task.FromResult(services.BuildServiceProvider().GetRequiredService<IConnectionMultiplexer>());
            options.InstanceName = instanceName;
        });

        // Register the Redis cache store as a keyed singleton.
        services.AddKeyedSingleton<ICacheStorage>(
            CacheStore.Distributed,
            (sp, key) => new DistributedCacheStorage(sp.GetRequiredService<IDistributedCache>()));

        if (cacheStrategies?.Length > 0)
        {
            foreach (var strategy in cacheStrategies)
            {
                services.UseCacheStrategy(CacheStore.Redis, strategy);
                services.UseCacheStrategy(CacheStore.Distributed, strategy);
            }
        }
    }

    /// <summary>
    ///     Registers a cache strategy implementation for a given cache store in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="store">The cache store type.</param>
    /// <param name="strategy">The cache strategy to register.</param>
    private static void UseCacheStrategy(
        this IServiceCollection services,
        CacheStore store,
        CacheStrategy strategy)
    {
        switch (strategy)
        {
            case CacheStrategy.CacheAside:
                services.AddKeyedSingleton<ICacheStrategy>((store, strategy), (sp, key) =>
                    new CacheAsideStrategy(
                        sp.GetRequiredKeyedService<ICacheStorage>(store),
                        sp.GetRequiredService<IDataStorage>()));
                break;
            case CacheStrategy.WriteThrough:
                services.AddKeyedSingleton<ICacheStrategy>((store, strategy), (sp, key) =>
                    new WriteThroughStrategy(
                        sp.GetRequiredKeyedService<ICacheStorage>(store),
                        sp.GetRequiredService<IDataStorage>()));
                break;
            case CacheStrategy.ReadThrough:
                services.AddKeyedSingleton<ICacheStrategy>((store, strategy), (sp, key) =>
                    new ReadThroughStrategy(
                        sp.GetRequiredKeyedService<ICacheStorage>(store),
                        sp.GetRequiredService<IDataStorage>()));
                break;
            case CacheStrategy.WriteBack:
                services.AddKeyedSingleton<ICacheStrategy>((store, strategy), (sp, key) =>
                    new WriteBackStrategy(
                        sp.GetRequiredKeyedService<ICacheStorage>(store),
                        sp.GetRequiredService<IDataStorage>()));
                break;
            case CacheStrategy.WriteAround:
                services.AddKeyedSingleton<ICacheStrategy>((store, strategy), (sp, key) =>
                    new WriteAroundStrategy(
                        sp.GetRequiredKeyedService<ICacheStorage>(store),
                        sp.GetRequiredService<IDataStorage>()));
                break;
        }
    }
}
