namespace KISS.QueryBuilder.Tests.DataSeeding;

public class WeatherDbContext(DbContextOptions<WeatherDbContext> options) : DbContext(options)
{
    public DbSet<Location> Locations { get; set; }
    public DbSet<DailyWeather> DailyWeathers { get; set; }
    public DbSet<Astronomy> Astronomies { get; set; }
    public DbSet<HourlyWeather> HourlyWeathers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>().ToTable("Location");
        modelBuilder.Entity<DailyWeather>().ToTable("DailyWeather");
        modelBuilder.Entity<Astronomy>().ToTable("Astronomy");
        modelBuilder.Entity<HourlyWeather>().ToTable("HourlyWeather");
    }
}
