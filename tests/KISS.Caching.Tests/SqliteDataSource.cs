namespace KISS.Caching.Tests;

public class SqliteDataSource : IDataStorage, IDisposable
{
    private MemoryDbContext DbContext { get; }

    public SqliteDataSource(MemoryDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        DbContext.Database.EnsureCreated();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var item = await DbContext.Products.FindAsync(key);
        if (item == null)
        {
            return default;
        }

        var serializedValue = MessagePackSerializer.Serialize(item);
        return MessagePackSerializer.Deserialize<T>(serializedValue);
    }

    public async Task UpdateAsync<T>(string key, T value)
    {
        var item = await DbContext.Products.FindAsync(key);
        if (item == null)
        {
            var serializedValue = MessagePackSerializer.Serialize(value);
            var newItem = MessagePackSerializer.Deserialize<Product>(serializedValue);
            DbContext.Products.Add(newItem);
        }
        else
        {
            var serializedValue = MessagePackSerializer.Serialize(value);
            var newItem = MessagePackSerializer.Deserialize<Product>(serializedValue);
            item = newItem;
            DbContext.Update(item);
        }

        await DbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}
