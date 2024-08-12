namespace KISS.QueryBuilder.Tests.Sqlite;

public sealed class SqliteTestsFixture : IAsyncLifetime
{
    private const string ConnectionString = "DataSource=:memory:";

    public SqliteConnection Connection { get; private set; } = default!;

    private ApplicationDbContext Context { get; set; } = default!;

    public async Task InitializeAsync()
    {
        await InitialiseDbConnectionAsync();
        await CreateSchemaAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        Connection.Close();
        await Connection.DisposeAsync();
    }

    private static SqliteConnection CreateDbConnection()
        => new(ConnectionString);

    private async Task InitialiseDbConnectionAsync()
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/dapper-limitations
        SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
        SqlMapper.AddTypeHandler(new GuidHandler());
        SqlMapper.AddTypeHandler(new TimeSpanHandler());

        Connection = CreateDbConnection();
        await Connection.OpenAsync();
    }

    private async Task CreateSchemaAsync()
    {
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(Connection)
            .Options;

        Context = new ApplicationDbContext(options);
        await Context.Database.EnsureCreatedAsync();
    }
}