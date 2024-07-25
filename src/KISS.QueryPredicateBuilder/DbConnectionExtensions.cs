namespace KISS.QueryPredicateBuilder;

public static class DbConnectionExtensions
{
    public static IList<TEntity> Gets<TEntity>(this DbConnection dbConnection, params IComponent[] components)
    {
        if (components.Any())
        {
            QueryBuilder builder = new();
            var (sql, parameters) = builder.Operation<TEntity>(components);
            return dbConnection.Query<TEntity>(sql, parameters).ToList();
        }
        else
        {
            Type entity = typeof(TEntity);
            string[] propsName = entity.GetProperties().Select(p => p.Name).ToArray();
            string columns = string.Join(", ", propsName);
            string table = entity.Name;
            string query = $"SELECT {columns} FROM {table}s";

            return dbConnection.Query<TEntity>(query).ToList();
        }
    }

    public static int Count<TEntity>(this DbConnection dbConnection)
    {
        Type entity = typeof(TEntity);
        string table = entity.Name;
        string query = $"SELECT COUNT(1) FROM {table}s";

        return dbConnection.ExecuteScalar<int>(query);
    }
}