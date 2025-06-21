namespace KISS.Caching.Tests;

public class RedisStorageTests : IDisposable
{
    private SqliteConnection Connection { get; init; }
    private ServiceProvider Services { get; init; }
    private CacheMechanismOptions Options { get; } = new(TimeSpan.FromMinutes(5), null);

    public RedisStorageTests()
    {
        Connection = new SqliteConnection("DataSource=:memory:");
        Connection.Open();

        var services = new ServiceCollection();

        // Register dependencies
        services.AddDbContext<MemoryDbContext>(options =>
           options.UseSqlite(Connection), ServiceLifetime.Scoped);
        services.AddScoped<IDataStorage, SqliteDataSource>();

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();

        services.UseRedis(
            configuration,
            "Redis",
            "Redis",
            CacheStrategy.CacheAside,
            CacheStrategy.WriteThrough,
            CacheStrategy.WriteBack,
            CacheStrategy.ReadThrough,
            CacheStrategy.WriteAround);

        Services = services.BuildServiceProvider();
    }

    private CacheStorage CreateCacheStorage(ICacheStrategy strategy) => new(strategy);

    public static TheoryData<CacheStore, CacheStrategy> AllStrategyOperationCombinations()
    {
        var data = new TheoryData<CacheStore, CacheStrategy>();
        CacheStore[] stores = { CacheStore.Redis, CacheStore.Distributed };
        CacheStrategy[] strategies = { CacheStrategy.CacheAside, CacheStrategy.WriteThrough, CacheStrategy.WriteBack, CacheStrategy.ReadThrough, CacheStrategy.WriteAround };

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
    public async Task GetOrSetAsync_CacheHit_ReturnsCachedValue(CacheStore storageType, CacheStrategy strategyType)
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
    public async Task CacheAsideStrategy_GetOrSetAsync_CacheMiss_FetchesFromDataSourceAndCaches()
    {
        // Arrange
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((CacheStore.Redis, CacheStrategy.CacheAside));
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
    public async Task ReadThroughStrategy_GetOrSetAsync_DataSourceAndCacheMiss_FetchesFromDataSourceAndCaches()
    {
        // Arrange
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((CacheStore.Redis, CacheStrategy.ReadThrough));
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
    public async Task ReadThroughStrategy_GetOrSetAsync_CacheMiss_FetchesFromDataSourceAndCaches()
    {
        // Arrange
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((CacheStore.Redis, CacheStrategy.ReadThrough));
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
    public async Task WriteThroughStrategy_UpdateAsync_WritesToCacheAndDataSource()
    {
        // Arrange
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((CacheStore.Redis, CacheStrategy.WriteThrough));
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
    public async Task WriteBackStrategy_UpdateAsync_WritesToCacheAndQueuesDataSourceUpdate()
    {
        // Arrange
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((CacheStore.Redis, CacheStrategy.WriteBack));
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
    public async Task WriteAroundStrategy_UpdateAsync_WritesToDataSourceOnly()
    {
        // Arrange
        var operation = Services.GetRequiredKeyedService<ICacheStrategy>((CacheStore.Redis, CacheStrategy.WriteAround));
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
