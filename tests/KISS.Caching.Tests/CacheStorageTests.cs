namespace KISS.Caching.Tests;

public class CacheStorageTests : IDisposable
{
    private SqliteConnection Connection { get; init; }
    private ServiceProvider Services { get; init; }

    public CacheStorageTests()
    {
        Connection = new SqliteConnection("DataSource=:memory:");
        Connection.Open();

        var services = new ServiceCollection();

        // Register dependencies
        services.AddMemoryCache();
        services.AddDistributedMemoryCache(); // Memory-based IDistributedCache
        services.AddDbContext<MemoryDbContext>(options =>
           options.UseSqlite(Connection), ServiceLifetime.Scoped);
        services.AddScoped<IDataStorage, SqliteDataSource>();

        // Register strategies
        services.AddScoped<InMemoryCacheStrategy>();
        services.AddScoped<DistributedCacheStrategy>();
        // services.AddSingleton<ICacheStorage, DistributedCacheStrategy>();
        // services.AddSingleton<ICacheStorage, FileBasedCacheStrategy>();
        // services.AddSingleton<ICacheStorage, HybridCacheStrategy>();

        // Register operations as factories to allow strategy injection
        services.AddKeyedScoped<ICacheOperation>("InMemory_CacheAside", (sp, key) =>
           new CacheAsideOperation(sp.GetRequiredService<InMemoryCacheStrategy>(), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheOperation>("InMemory_WriteThrough", (sp, key) =>
            new WriteThroughOperation(sp.GetRequiredService<InMemoryCacheStrategy>(), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheOperation>("InMemory_WriteBack", (sp, key) =>
            new WriteBackOperation(sp.GetRequiredService<InMemoryCacheStrategy>(), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheOperation>("InMemory_ReadThrough", (sp, key) =>
            new ReadThroughOperation(sp.GetRequiredService<InMemoryCacheStrategy>(), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheOperation>("InMemory_WriteAround", (sp, key) =>
            new WriteAroundOperation(sp.GetRequiredService<InMemoryCacheStrategy>(), sp.GetRequiredService<IDataStorage>()));

        services.AddKeyedScoped<ICacheOperation>("Distributed_CacheAside", (sp, key) =>
            new CacheAsideOperation(sp.GetRequiredService<DistributedCacheStrategy>(), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheOperation>("Distributed_WriteThrough", (sp, key) =>
            new WriteThroughOperation(sp.GetRequiredService<DistributedCacheStrategy>(), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheOperation>("Distributed_WriteBack", (sp, key) =>
            new WriteBackOperation(sp.GetRequiredService<DistributedCacheStrategy>(), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheOperation>("Distributed_ReadThrough", (sp, key) =>
            new ReadThroughOperation(sp.GetRequiredService<DistributedCacheStrategy>(), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheOperation>("Distributed_WriteAround", (sp, key) =>
            new WriteAroundOperation(sp.GetRequiredService<DistributedCacheStrategy>(), sp.GetRequiredService<IDataStorage>()));

        Services = services.BuildServiceProvider();
    }

    private CacheStorage CreateCacheStorage(ICacheOperation operation)
    {
        return new CacheStorage(operation);
    }

    private static string GetOperationKey(Type strategyType, Type operationType)
    {
        string strategyName = strategyType.Name.Replace("CacheStrategy", "");
        string operationName = operationType.Name.Replace("Operation", "");
        return $"{strategyName}_{operationName}";
    }

    public static TheoryData<Type, Type> AllStrategyOperationCombinations()
    {
        var data = new TheoryData<Type, Type>();
        Type[] strategies = { typeof(InMemoryCacheStrategy) };
        Type[] operations = { typeof(CacheAsideOperation), typeof(WriteThroughOperation), typeof(WriteBackOperation), typeof(ReadThroughOperation), typeof(WriteAroundOperation) };

        foreach (var strategy in strategies)
        {
            foreach (var operation in operations)
            {
                data.Add(strategy, operation);
            }
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(AllStrategyOperationCombinations))]
    public async Task GetOrSetAsync_CacheHit_ReturnsCachedValue(Type strategyType, Type operationType)
    {
        // Arrange
        // using var scope = Services.CreateScope();
        // var strategy = (ICacheStorage)Services.GetRequiredService(strategyType);
        var operation = Services.GetKeyedService<ICacheOperation>(GetOperationKey(strategyType, operationType));
        var storage = CreateCacheStorage(operation!);
        var key = $"key_{strategyType.Name}_{operationType.Name}";
        Product expectedValue = new() { Key = key, Value = "cached-value" };
        var options = new CacheMechanismOptions(TimeSpan.FromMinutes(5), null);

        // await strategy.SetAsync(key, expectedValue, options);

        // Act
        var result = await storage.GetOrSetAsync(key, expectedValue, options);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(expectedValue.Value, result.Value!.Value);
    }

    public void Dispose()
    {
        Connection.Dispose();
        Services.Dispose();
        GC.SuppressFinalize(this);
    }
}