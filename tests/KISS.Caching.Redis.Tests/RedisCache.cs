namespace KISS.Caching.Redis.Tests;

[MessagePackObject]
public class User
{
    [MessagePack.KeyAttribute(0)]
    public int Id { get; set; }

    [MessagePack.KeyAttribute(1)]
    public string? Name { get; set; }
}

public interface ISqliteDatabase
{
    Task<User> GetUserAsync(int userId);
    Task UpdateUserAsync(User user);
    void Dispose();
}

public class SqliteDatabase : ISqliteDatabase, IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var command = _connection.CreateCommand();
        command.CommandText = "CREATE TABLE Users (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL)";
        command.ExecuteNonQuery();
    }

    public async Task<User> GetUserAsync(int userId)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT Id, Name FROM Users WHERE Id = $id";
        command.Parameters.AddWithValue("$id", userId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User { Id = reader.GetInt32(0), Name = reader.GetString(1) };
        }

        return new User { Id = userId, Name = $"User_{userId}" };
    }

    public async Task UpdateUserAsync(User user)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
                INSERT OR REPLACE INTO Users (Id, Name)
                VALUES ($id, $name)";
        command.Parameters.AddWithValue("$id", user.Id);
        command.Parameters.AddWithValue("$name", user.Name);
        await command.ExecuteNonQueryAsync();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}

public class RedisCache : IDisposable
{
    private IRedisConnection Redis { get; }
    private ISqliteDatabase Database { get; }
    private static RedisChannel PubSubChannel { get; } = RedisChannel.Literal("user_updates");
    private TimeSpan CacheDuration { get; } = TimeSpan.FromSeconds(30);

    public RedisCache(IRedisConnection redis, ISqliteDatabase database)
    {
        Redis = redis;
        Database = database;
        Redis.Subscriber.Subscribe(PubSubChannel, async (channel, message) =>
            {
                string? cacheKey = message;
                if (!string.IsNullOrEmpty(cacheKey))
                {
                    await Redis.DeleteAsync(cacheKey);
                }
            });
    }

    // Cache-Aside
    public async Task<User> GetUserCacheAsideAsync(int userId)
    {
        string cacheKey = $"User_{userId}";
        User? cachedValue = await Redis.GetAsync<User>(cacheKey);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        User user = await Database.GetUserAsync(userId);
        await Redis.SetAsync(cacheKey, user, CacheDuration);
        return user;
    }

    // Read-Through
    public async Task<User> GetUserReadThroughAsync(int userId)
    {
        string cacheKey = $"User_{userId}";
        User? cachedValue = await Redis.GetAsync<User>(cacheKey);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        User user = await Database.GetUserAsync(userId);
        await Redis.SetAsync(cacheKey, user, CacheDuration);
        return user;
    }

    // Write-Through
    public async Task WriteThroughUpdateAsync(User user)
    {
        string cacheKey = $"User_{user.Id}";
        await Database.UpdateUserAsync(user);
        await Redis.SetAsync(cacheKey, user, CacheDuration);
        await Redis.Subscriber.PublishAsync(PubSubChannel, cacheKey);
    }

    // Write-Back
    public async Task WriteBackUpdateAsync(User user)
    {
        string cacheKey = $"User_{user.Id}";
        await Redis.SetAsync(cacheKey, user, CacheDuration);
        await Redis.Subscriber.PublishAsync(PubSubChannel, cacheKey);
        _ = Task.Run(() => Database.UpdateUserAsync(user));
    }

    // Write-Around
    public async Task WriteAroundUpdateAsync(User user)
    {
        string cacheKey = $"User_{user.Id}";
        await Database.UpdateUserAsync(user);
        await Redis.DeleteAsync(cacheKey);
        await Redis.Subscriber.PublishAsync(PubSubChannel, cacheKey);
    }

    // Cache Invalidation
    public async Task InvalidateUserCacheAsync(int userId)
    {
        string cacheKey = $"User_{userId}";
        await Redis.DeleteAsync(cacheKey);
        await Redis.Subscriber.PublishAsync(PubSubChannel, cacheKey);
    }

    public void Dispose()
    {
        // _redis.Dispose();
        Database.Dispose();
    }
}