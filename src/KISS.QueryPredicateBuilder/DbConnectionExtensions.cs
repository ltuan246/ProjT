namespace KISS.QueryPredicateBuilder;

public static class DbConnectionExtensions
{
    public static void Query<TEntity>(this DbConnection dbConnection, params IComponent[] components)
    {
        QueryBuilder builder = new();
        builder.SelectFrom<TEntity>();
        var sql = builder.Operation(components);
    }
}