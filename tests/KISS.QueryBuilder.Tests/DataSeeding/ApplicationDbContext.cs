namespace KISS.QueryBuilder.Tests.DataSeeding;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // https://dev.to/connerphillis/sequential-guids-in-entity-framework-core-might-not-be-sequential-3408
        // modelBuilder.Entity<User>().Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
        modelBuilder.Entity<User>().HasData(
            new User { Id = Guid.NewGuid(), Name = "Tuna" },
            new User { Id = Guid.NewGuid(), Name = "Tuan" }
        );
    }
}