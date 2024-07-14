namespace KISS.QueryPredicateBuilder;

public static class DbConnectionExtensions
{
    public static List<TEntity> Gets<TEntity>(this DbConnection dbConnection, params IComponent[] components)
    {
        QueryBuilder builder = new();
        var (sql, parameters) = builder.Operation<TEntity>(components);
        return dbConnection.Query<TEntity>(sql, parameters).ToList();
    }
}