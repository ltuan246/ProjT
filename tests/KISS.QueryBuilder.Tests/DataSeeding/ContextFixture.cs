namespace KISS.QueryBuilder.Tests.DataSeeding;

public class ContextFixture
{
    public static SqliteConnection CreateConnection()
    {
        SqliteConnection connection = new("DataSource=:memory:");
        connection.Open();
        return connection;
    }

    public static DbContextOptions<ApplicationDbContext> CreateOptions(SqliteConnection sqliteConnection)
        => new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(sqliteConnection)
            .EnableSensitiveDataLogging(true)
            .Options;

    public static ApplicationDbContext CreateContext(SqliteConnection sqliteConnection)
    {
        DbContextOptions<ApplicationDbContext> dbContextOptions = CreateOptions(sqliteConnection);

        ApplicationDbContext context = new(dbContextOptions);
        context.Database.EnsureCreated();

        return context;
    }
}