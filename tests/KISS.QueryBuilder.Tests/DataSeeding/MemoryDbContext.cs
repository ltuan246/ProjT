namespace KISS.QueryBuilder.Tests.DataSeeding;

public class MemoryDbContext(DbContextOptions<MemoryDbContext> options) : DbContext(options)
{
    public DbSet<MemoryLocation> Locations { get; set; }
    public DbSet<MemoryDailyWeather> DailyWeathers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MemoryLocation>().ToTable("Location");
        modelBuilder.Entity<MemoryDailyWeather>().ToTable("DailyWeather");

        const string locationsFile = "Assets/locations.csv";
        const string dailyWeathersFile = "Assets/daily_weather.csv";

        var locations = CsvAssists.FromCsv<MemoryLocation>(locationsFile);
        var dailyWeathers = CsvAssists.FromCsv<MemoryDailyWeather>(dailyWeathersFile);

        var dateTimeConverter = new ValueConverter<DateTime, string>(
            v => v.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
            v => DateTimeOffset.Parse(v).UtcDateTime // Matches ChangeType
        );

        modelBuilder.Entity<MemoryDailyWeather>(entity =>
        {
            // Ensure Date is stored as UTC in SQLite (stored as ISO 8601 string)
            entity.Property(e => e.Date)
                .HasConversion(dateTimeConverter)
                .HasColumnType("TEXT");
        });

        modelBuilder.Entity<MemoryLocation>().HasData(locations);
        modelBuilder.Entity<MemoryDailyWeather>().HasData(dailyWeathers);
    }
}
