namespace KISS.QueryBuilder.Tests.DataSeeding;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Weather> Weathers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // https://dev.to/connerphillis/sequential-guids-in-entity-framework-core-might-not-be-sequential-3408
        // modelBuilder.Entity<User>().Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

        const string fileName = "Assets/GlobalWeatherRepository.csv";
        var weathers = CsvAssists.FromCsv<Weather>(fileName);

        modelBuilder.Entity<Weather>().HasData(weathers);
    }
}