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
        services.AddDbContext<MemoryDbContext>(options =>
           options.UseSqlite(Connection), ServiceLifetime.Scoped);
        services.AddScoped<IDataStorage, SqliteDataSource>();

        services.UseCacheStore<InMemoryCacheStorage>(
            CacheStrategies.CacheAside,
            CacheStrategies.WriteThrough,
            CacheStrategies.WriteBack,
            CacheStrategies.ReadThrough,
            CacheStrategies.WriteAround);

        services.UseCacheStore<DistributedCacheStorage>(
            CacheStrategies.CacheAside,
            CacheStrategies.WriteThrough,
            CacheStrategies.WriteBack,
            CacheStrategies.ReadThrough,
            CacheStrategies.WriteAround);

        Services = services.BuildServiceProvider();
    }

    private CacheStorage CreateCacheStorage(ICacheStrategy strategy) => new(strategy);

    public static TheoryData<CacheStores, CacheStrategies> AllStrategyOperationCombinations()
    {
        var data = new TheoryData<CacheStores, CacheStrategies>();
        CacheStores[] stores = { CacheStores.InMemory, CacheStores.Distributed };
        CacheStrategies[] strategies = { CacheStrategies.CacheAside, CacheStrategies.WriteThrough, CacheStrategies.WriteBack, CacheStrategies.ReadThrough, CacheStrategies.WriteAround };

        foreach (var store in stores)
        {
            foreach (var operation in strategies)
            {
                data.Add(store, operation);
            }
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(AllStrategyOperationCombinations))]
    public async Task GetOrSetAsync_CacheHit_ReturnsCachedValue(CacheStores storageType, CacheStrategies strategyType)
    {
        // Arrange
        var strategy = Services.GetRequiredKeyedService<ICacheStorage>(storageType);
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((storageType, strategyType));
        var storage = CreateCacheStorage(operation!);
        var key = $"key_{storageType}_{strategyType}";
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
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((CacheStores.InMemory, CacheStrategies.CacheAside));
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
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((CacheStores.InMemory, CacheStrategies.ReadThrough));
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
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((CacheStores.InMemory, CacheStrategies.ReadThrough));
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
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((CacheStores.InMemory, CacheStrategies.WriteThrough));
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
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((CacheStores.InMemory, CacheStrategies.WriteBack));
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
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((CacheStores.InMemory, CacheStrategies.WriteAround));
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
        Connection.Close();
        Connection.Dispose();
        Services.Dispose();
        GC.SuppressFinalize(this);
    }
}
