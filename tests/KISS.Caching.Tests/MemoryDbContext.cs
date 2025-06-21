namespace KISS.Caching.Tests;

public class MemoryDbContext(DbContextOptions<MemoryDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().ToTable("Products");
    }
}

[MessagePackObject]
public class Product
{
    [System.ComponentModel.DataAnnotations.Key]
    [MessagePack.Key(0)]
    public string Key { get; set; } = string.Empty;

    [MessagePack.Key(1)]
    public string Value { get; set; } = string.Empty;
}