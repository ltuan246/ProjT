namespace KISS.QueryBuilder.Tests.DataSeeding;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Weather> Weathers { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<DustCost> DustCosts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // https://dev.to/connerphillis/sequential-guids-in-entity-framework-core-might-not-be-sequential-3408
        // modelBuilder.Entity<User>().Property(e => e.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

        const string weatherFile = "Assets/Weather.csv";
        const string cardsFile = "Assets/HearthstoneCards/cards.csv";
        const string dustCostsFile = "Assets/HearthstoneCards/dust_costs.csv";

        var weathers = CsvAssists.FromCsv<Weather>(weatherFile);
        var cards = CsvAssists.FromCsv<Card>(cardsFile);
        var dustCosts = CsvAssists.FromCsv<DustCost>(dustCostsFile);

        modelBuilder.Entity<Weather>().HasData(weathers);
        modelBuilder.Entity<Card>().HasData(cards);
        modelBuilder.Entity<DustCost>().HasData(dustCosts);
    }
}