namespace KISS.QueryBuilder.DataAccess;

public record DapperContext(IOptions<DbContextOptions> Options)
{
    public IDbConnection CreateConnection()
        => new SqliteConnection(Options.Value.SQLiteConnection);
}