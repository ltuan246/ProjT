namespace KISS.QueryBuilder.Tests.Sqlite;

public sealed class SqliteTestsFixture : IAsyncLifetime
{
    private const string ConnectionString = "DataSource=:memory:";

    public SqliteConnection Connection { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await InitialiseDbConnectionAsync();
        await CreateSchemaAsync();
    }

    public async Task DisposeAsync()
    {
        Connection.Close();
        await Connection.DisposeAsync();
    }

    private static SqliteConnection CreateDbConnection()
        => new(ConnectionString);

    private async Task InitialiseDbConnectionAsync()
    {
        Connection = CreateDbConnection();
        await Connection.OpenAsync();
    }

    private async Task CreateSchemaAsync()
    {
        await Task.CompletedTask;
    }
}