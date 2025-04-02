namespace KISS.QueryBuilder.Tests.Sqlite;

public sealed class SqliteTestsFixture : IAsyncLifetime
{
    public SqliteConnection Connection { get; private set; } = default!;

    private WeatherDbContext Context { get; set; } = default!;

    private bool IsDisposed { get; set; }

    public async Task InitializeAsync()
    {
        await InitialiseDbConnectionAsync();
        await CreateSchemaAsync();
    }

    public async Task DisposeAsync()
    {
        if (!IsDisposed)
        {
            // await Context.Database.EnsureDeletedAsync();
            Connection.Close();
            await Connection.DisposeAsync();
            IsDisposed = true;
        }
    }

    private static SqliteConnection CreateDbConnection()
    {
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "weather.db");
        string connectionString = $"DataSource={dbPath};Mode=ReadWrite;Cache=Shared";
        return new(connectionString);
    }

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
        DbContextOptions<WeatherDbContext> options = new DbContextOptionsBuilder<WeatherDbContext>()
            .UseSqlite(Connection)
            .Options;

        Context = new WeatherDbContext(options);
        await Context.Database.EnsureCreatedAsync();
    }
}

public sealed class MemoryDbTestsFixture : IAsyncLifetime
{
    public SqliteConnection Connection { get; private set; } = default!;

    private WeatherDbContext Context { get; set; } = default!;

    private bool IsDisposed { get; set; }

    public async Task InitializeAsync()
    {
        await InitialiseDbConnectionAsync();
        await CreateSchemaAsync();
    }

    public async Task DisposeAsync()
    {
        if (!IsDisposed)
        {
            // await Context.Database.EnsureDeletedAsync();
            Connection.Close();
            await Connection.DisposeAsync();
            IsDisposed = true;
        }
    }

    private static SqliteConnection CreateDbConnection()
    {
        const string connectionString = "Data Source=:memory:";
        return new(connectionString);
    }

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
        DbContextOptions<WeatherDbContext> options = new DbContextOptionsBuilder<WeatherDbContext>()
            .UseSqlite(Connection)
            .Options;

        Context = new WeatherDbContext(options);
        await Context.Database.EnsureCreatedAsync();
    }
}
