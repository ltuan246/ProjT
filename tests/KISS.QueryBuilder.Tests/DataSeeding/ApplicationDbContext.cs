namespace KISS.QueryBuilder.Tests.DataSeeding;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = Guid.NewGuid().ToString(), Name = "Tuna" }
        );
    }
}