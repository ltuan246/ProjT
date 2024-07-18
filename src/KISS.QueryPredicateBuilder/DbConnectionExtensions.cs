namespace KISS.QueryPredicateBuilder;

public static class DbConnectionExtensions
{
    public static List<TEntity> Gets<TEntity>(this DbConnection dbConnection, params IComponent[] components)
    {
        if (components.Any())
        {
            QueryBuilder builder = new();
            var (sql, parameters) = builder.Operation<TEntity>(components);
            return dbConnection.Query<TEntity>(sql, parameters).ToList();
        }
        else
        {
            var entity = typeof(TEntity);
            const string sqlSelectClause = "SELECT {0} FROM {1}s";
            string[] propsName = entity.GetProperties().Select(p => p.Name).ToArray();
            string columns = string.Join(", ", propsName);
            string table = entity.Name;

            StringBuilder builder = new();
            builder.AppendFormat(sqlSelectClause, columns, table);
            string query = builder.ToString();

            return dbConnection.Query<TEntity>(query).ToList();
        }
    }

    public static int Count<TEntity>(this DbConnection dbConnection)
    {
        var entity = typeof(TEntity);
        const string sqlCount = "SELECT COUNT(1) FROM {0}s";
        string table = entity.Name;

        StringBuilder builder = new();
        builder.AppendFormat(sqlCount, table);
        string query = builder.ToString();

        return dbConnection.ExecuteScalar<int>(query);
    }
}