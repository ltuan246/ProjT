namespace KISS.QueryPredicateBuilder;

public static class DbConnectionExtensions
{
    public static void Query<TEntity>(this DbConnection dbConnection, params IComponent[] components)
    {
        QueryBuilder builder = new();
        var sql = builder.Operation(components);
    }
}