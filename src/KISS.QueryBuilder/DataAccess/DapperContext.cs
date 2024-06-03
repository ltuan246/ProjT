namespace KISS.QueryBuilder.DataAccess;

public record DapperContext(IOptions<DbContextOptions> Options)
{
    public IDbConnection CreateConnection()
        => new SqlConnection(Options.Value.DefaultConnection);
}