namespace KISS.QueryBuilder.DataAccess;

public class DbContextOptions
{
    public required string DefaultConnection { get; init; }
    public required string SQLiteConnection { get; init; }
}