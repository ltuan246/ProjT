namespace KISS.Caching.Redis.Tests;

public class CachingStrategiesTest : IDisposable
{
    private ServiceProvider Services { get; init; }

    private RedisCache Caching { get; }
    private ISqliteDatabase Database { get; }
    private IRedisConnection RedisConnection { get; }
    private static RedisChannel PubSubChannel { get; } = RedisChannel.Literal("user_updates");

    public CachingStrategiesTest()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();

        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.UseRedis(configuration);
        serviceCollection.AddScoped<RedisCache>();
        serviceCollection.AddScoped<ISqliteDatabase, SqliteDatabase>();
        Services = serviceCollection.BuildServiceProvider();

        RedisConnection = Services.GetRequiredService<IRedisConnection>();
        Caching = Services.GetRequiredService<RedisCache>();
        Database = Services.GetRequiredService<ISqliteDatabase>();

        RedisConnection.Db.Execute("FLUSHDB");
    }

    [Fact]
    public async Task GetUserCacheAsideAsync_CacheMiss_FetchesFromDatabaseAndCachesResult()
    {
        int userId = 1;
        var user = new User { Id = userId, Name = "User_1" };
        await Database.UpdateUserAsync(user);

        var result = await Caching.GetUserCacheAsideAsync(userId);

        Assert.Equal(user.Name, result.Name);
        var cachedValue = await RedisConnection.GetAsync<User>($"User_{userId}");
        Assert.Equal(user.Name, cachedValue!.Name);
    }

    [Fact]
    public async Task GetUserCacheAsideAsync_CacheHit_ReturnsCachedData()
    {
        int userId = 1;
        var user = new User { Id = userId, Name = "User_1" };
        await RedisConnection.SetAsync($"User_{userId}", user);

        var result = await Caching.GetUserCacheAsideAsync(userId);

        Assert.Equal(user.Name, result.Name);
        var dbUser = await Database.GetUserAsync(userId);
        Assert.Equal($"User_{userId}", dbUser.Name);
    }

    [Fact]
    public async Task GetUserReadThroughAsync_CacheMiss_FetchesFromDatabaseAndCachesResult()
    {
        int userId = 1;
        var user = new User { Id = userId, Name = "User_1" };
        await Database.UpdateUserAsync(user);

        var result = await Caching.GetUserReadThroughAsync(userId);

        Assert.Equal(user.Name, result.Name);
        var cachedValue = await RedisConnection.GetAsync<User>($"User_{userId}");
        Assert.Equal(user.Name, cachedValue!.Name);
    }

    [Fact]
    public async Task WriteThroughUpdateAsync_UpdatesCacheAndDatabase()
    {
        int userId = 1;
        var user = new User { Id = userId, Name = "John Doe" };

        await Caching.WriteThroughUpdateAsync(user);
        var result = await Caching.GetUserCacheAsideAsync(userId);

        Assert.Equal(user.Name, result.Name);
        var dbUser = await Database.GetUserAsync(userId);
        Assert.Equal(user.Name, dbUser.Name);
        var cachedValue = await RedisConnection.GetAsync<User>($"User_{userId}");
        Assert.Equal(user.Name, cachedValue!.Name);
    }

    [Fact]
    public async Task WriteBackUpdateAsync_UpdatesCacheImmediatelyAndEventuallyDatabase()
    {
        int userId = 1;
        var user = new User { Id = userId, Name = "Jane Doe" };

        await Caching.WriteBackUpdateAsync(user);
        await Task.Delay(100);
        var result = await Caching.GetUserCacheAsideAsync(userId);

        Assert.Equal(user.Name, result.Name);
        var cachedValue = await RedisConnection.GetAsync<User>($"User_{userId}");
        Assert.Equal(user.Name, cachedValue!.Name);
        await Task.Delay(100);
        var dbUser = await Database.GetUserAsync(userId);
        Assert.Equal(user.Name, dbUser.Name);
    }

    [Fact]
    public async Task WriteAroundUpdateAsync_UpdatesDatabaseNotCache()
    {
        int userId = 1;
        var initialUser = new User { Id = userId, Name = "Initial" };
        await Caching.WriteThroughUpdateAsync(initialUser);
        var updatedUser = new User { Id = userId, Name = "Updated" };

        await Caching.WriteAroundUpdateAsync(updatedUser);
        var result = await Caching.GetUserCacheAsideAsync(userId);

        var cachedValue = await RedisConnection.GetAsync<User>($"User_{userId}");
        Assert.NotNull(cachedValue);
        Assert.Equal(updatedUser.Name, result.Name);
        var dbUser = await Database.GetUserAsync(userId);
        Assert.Equal(updatedUser.Name, dbUser.Name);
    }

    [Fact]
    public async Task InvalidateUserCacheAsync_RemovesCachedData()
    {
        int userId = 1;
        var user = new User { Id = userId, Name = "User_1" };
        await Caching.WriteThroughUpdateAsync(user);

        await Caching.InvalidateUserCacheAsync(userId);
        await Task.Delay(100);
        var result = await Caching.GetUserCacheAsideAsync(userId);

        var cachedValue = await RedisConnection.GetAsync<User>($"User_{userId}");
        Assert.NotNull(cachedValue);
        Assert.Equal($"User_{userId}", result.Name);
    }

    [Fact]
    public async Task InvalidateUserCacheAsync_PubSubInvalidation_TriggersAcrossInstances()
    {
        int userId = 1;
        var user = new User { Id = userId, Name = "User_1" };
        await Caching.WriteThroughUpdateAsync(user);

        bool invalidationTriggered = false;
        await RedisConnection.Subscriber.SubscribeAsync(PubSubChannel, (channel, message) =>
        {
            invalidationTriggered = true;
        });

        await Caching.InvalidateUserCacheAsync(userId);
        await Task.Delay(100);

        Assert.True(invalidationTriggered);
        var cachedValue = await RedisConnection.GetAsync<User>($"User_{userId}");
        Assert.Null(cachedValue);
        var result = await Caching.GetUserCacheAsideAsync(userId);
        Assert.Equal($"User_{userId}", result.Name);
    }

    public void Dispose()
    {
        Caching.Dispose();
        Database.Dispose();
        // _redis.Dispose();

        Services.Dispose();
        GC.SuppressFinalize(this);
    }
}
