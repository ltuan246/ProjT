namespace KISS.Caching.Tests;

public class CacheStorageTests : IDisposable
{
    private SqliteConnection Connection { get; init; }
    private ServiceProvider Services { get; init; }
    private CacheMechanismOptions Options { get; } = new(TimeSpan.FromMinutes(5), null);

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
        services.AddKeyedScoped<ICacheStorage>("InMemoryCacheStorage", (sp, key) =>
            new InMemoryCacheStorage(sp.GetRequiredService<IMemoryCache>()));
        services.AddKeyedScoped<ICacheStorage>("DistributedCacheStorage", (sp, key) =>
            new DistributedCacheStorage(sp.GetRequiredService<IDistributedCache>()));
        // services.AddSingleton<ICacheStorage, FileBasedCacheStrategy>();
        // services.AddSingleton<ICacheStorage, HybridCacheStrategy>();

        // Register operations as factories to allow strategy injection
        services.AddKeyedScoped<ICacheStrategy>("InMemory_CacheAside", (sp, key) =>
           new CacheAsideStrategy(sp.GetRequiredKeyedService<ICacheStorage>("InMemoryCacheStorage"), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheStrategy>("InMemory_WriteThrough", (sp, key) =>
            new WriteThroughStrategy(sp.GetRequiredKeyedService<ICacheStorage>("InMemoryCacheStorage"), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheStrategy>("InMemory_WriteBack", (sp, key) =>
            new WriteBackStrategy(sp.GetRequiredKeyedService<ICacheStorage>("InMemoryCacheStorage"), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheStrategy>("InMemory_ReadThrough", (sp, key) =>
            new ReadThroughStrategy(sp.GetRequiredKeyedService<ICacheStorage>("InMemoryCacheStorage"), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheStrategy>("InMemory_WriteAround", (sp, key) =>
            new WriteAroundStrategy(sp.GetRequiredKeyedService<ICacheStorage>("InMemoryCacheStorage"), sp.GetRequiredService<IDataStorage>()));

        services.AddKeyedScoped<ICacheStrategy>("Distributed_CacheAside", (sp, key) =>
            new CacheAsideStrategy(sp.GetRequiredKeyedService<ICacheStorage>("DistributedCacheStorage"), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheStrategy>("Distributed_WriteThrough", (sp, key) =>
            new WriteThroughStrategy(sp.GetRequiredKeyedService<ICacheStorage>("DistributedCacheStorage"), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheStrategy>("Distributed_WriteBack", (sp, key) =>
            new WriteBackStrategy(sp.GetRequiredKeyedService<ICacheStorage>("DistributedCacheStorage"), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheStrategy>("Distributed_ReadThrough", (sp, key) =>
            new ReadThroughStrategy(sp.GetRequiredKeyedService<ICacheStorage>("DistributedCacheStorage"), sp.GetRequiredService<IDataStorage>()));
        services.AddKeyedScoped<ICacheStrategy>("Distributed_WriteAround", (sp, key) =>
            new WriteAroundStrategy(sp.GetRequiredKeyedService<ICacheStorage>("DistributedCacheStorage"), sp.GetRequiredService<IDataStorage>()));

        Services = services.BuildServiceProvider();
    }

    private CacheStorage CreateCacheStorage(ICacheStrategy strategy)
    {
        return new CacheStorage(strategy);
    }

    private static string GetOperationKey(Type strategyType, Type operationType)
    {
        string strategyName = strategyType.Name.Replace("CacheStrategy", "");
        string operationName = operationType.Name.Replace("Strategy", "");
        return $"{strategyName}_{operationName}";
    }

    public static TheoryData<Type, Type> AllStrategyOperationCombinations()
    {
        var data = new TheoryData<Type, Type>();
        Type[] strategies = { typeof(InMemoryCacheStorage), typeof(DistributedCacheStorage) };
        Type[] operations = { typeof(CacheAsideStrategy), typeof(WriteThroughStrategy), typeof(WriteBackStrategy), typeof(ReadThroughStrategy), typeof(WriteAroundStrategy) };

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
        var strategy = Services.GetRequiredKeyedService<ICacheStorage>(strategyType.Name);
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>(GetOperationKey(strategyType, operationType));
        var storage = CreateCacheStorage(operation!);
        var key = $"key_{strategyType.Name}_{operationType.Name}";
        Product expectedValue = new() { Key = key, Value = "Initial" };
        await strategy.SetAsync(key, expectedValue, Options);

        // Act
        var result = await storage.GetOrSetAsync(key, expectedValue, Options);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(expectedValue.Value, result.Value!.Value);
    }

    [Fact]
    public async Task CacheAside_InMemory_GetOrSetAsync_CacheMiss_FetchesFromDataSourceAndCaches()
    {
        // Arrange
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>("InMemory_CacheAside");
        var storage = CreateCacheStorage(operation!);
        var key = "key_InMemoryCacheStrategy_CacheAside";
        Product expectedValue = new() { Key = key, Value = "Initial" };
        var options = new CacheMechanismOptions(TimeSpan.FromMinutes(5), null);

        // Act
        var result = await storage.GetOrSetAsync(key, expectedValue, options);

        var dbContext = Services.GetRequiredService<MemoryDbContext>();
        var isExisting = await dbContext.Products.AnyAsync(p => p.Key == key);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(expectedValue.Value, result.Value!.Value);
        Assert.False(isExisting);
    }

    [Fact]
    public async Task ReadThrough_InMemory_GetOrSetAsync_DataSourceAndCacheMiss_FetchesFromDataSourceAndCaches()
    {
        // Arrange
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>("InMemory_ReadThrough");
        var storage = CreateCacheStorage(operation!);
        var key = "key_InMemoryCacheStrategy_ReadThrough";
        Product expectedValue = new() { Key = key, Value = "Initial" };

        // Act
        var result = await storage.GetOrSetAsync(key, expectedValue, Options);
        var dbContext = Services.GetRequiredService<MemoryDbContext>();
        var isExisting = await dbContext.Products.AnyAsync(p => p.Key == key);

        // Assert
        Assert.False(result.HasValue);
        Assert.False(isExisting);
    }

    [Fact]
    public async Task ReadThrough_InMemory_GetOrSetAsync_CacheMiss_FetchesFromDataSourceAndCaches()
    {
        // Arrange
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>("InMemory_ReadThrough");
        var storage = CreateCacheStorage(operation!);
        var key = "key_InMemoryCacheStrategy_ReadThrough";
        Product expectedValue = new() { Key = key, Value = "Initial" };

        // Act
        var dbContext = Services.GetRequiredService<MemoryDbContext>();
        await dbContext.Products.AddAsync(expectedValue);
        await dbContext.SaveChangesAsync();
        var isExisting = await dbContext.Products.AnyAsync(p => p.Key == key);
        var result = await storage.GetOrSetAsync(key, expectedValue, Options);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(expectedValue.Value, result.Value!.Value);
    }

    [Fact]
    public async Task WriteThrough_InMemory_UpdateAsync_WritesToCacheAndDataSource()
    {
        // Arrange
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>("InMemory_WriteThrough");
        var storage = CreateCacheStorage(operation!);
        var key = "key_InMemoryCacheStrategy_WriteThrough";
        Product expectedValue = new() { Key = key, Value = "Initial" };

        // Act
        await storage.UpdateAsync(key, expectedValue, Options);
        var result = await storage.GetOrSetAsync<Product>(key, expectedValue, Options);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(expectedValue.Value, result.Value!.Value);
    }


    [Fact]
    public async Task WriteBack_InMemory_UpdateAsync_WritesToCacheAndQueuesDataSourceUpdate()
    {
        // Arrange
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>("InMemory_WriteBack");
        var storage = CreateCacheStorage(operation!);
        var key = "key_InMemoryCacheStrategy_WriteBack";
        Product expectedValue = new() { Key = key, Value = "Initial" };

        // Act
        await storage.UpdateAsync(key, expectedValue, Options);
        await Task.Delay(1000); // Wait for background task to process
        var result = await storage.GetOrSetAsync<Product>(key, expectedValue, Options);

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(expectedValue.Value, result.Value!.Value);
    }

    [Fact]
    public async Task WriteAround_InMemory_UpdateAsync_WritesToDataSourceOnly()
    {
        // Arrange
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>("InMemory_WriteAround");
        var storage = CreateCacheStorage(operation!);
        var key = "key_InMemoryCacheStrategy_WriteAround";
        Product initialValue = new() { Key = key, Value = "Initial" };
        Product updatedValue = new() { Key = key, Value = "Updated" };

        var initialCache = await storage.GetOrSetAsync<Product>(key, initialValue, Options);

        await storage.UpdateAsync<Product>(key, updatedValue, Options);

        // Act
        var dbContext = Services.GetRequiredService<MemoryDbContext>();
        var updatedProduct = await dbContext.Products.FindAsync(key);
        var updatedCache = await storage.GetOrSetAsync<Product>(key, updatedValue, Options);

        // Assert
        Assert.NotNull(updatedProduct);
        Assert.NotNull(updatedCache.Value);
        Assert.NotEqual(updatedProduct.Value, updatedCache.Value.Value);
    }

    public void Dispose()
    {
        Services.Dispose();
        GC.SuppressFinalize(this);
    }
}
