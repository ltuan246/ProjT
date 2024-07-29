namespace KISS.QueryPredicateBuilder;

/// <summary>
/// An extension class for database connections.
/// </summary>
public static class DbConnectionExtensions
{
    /// <summary>
    /// Retrieves data from a database based on conditions.
    /// </summary>
    /// <param name="dbConnection">The database connections.</param>
    /// <param name="components">The Query Builders.</param>
    /// <typeparam name="TEntity">The type of the record.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    public static IList<TEntity> Gets<TEntity>(this DbConnection dbConnection, params IComponent[] components)
    {
        components ??= [];
        if (components.Length != 0)
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

    /// <summary>
    /// Returns the total number of elements in a collection or the number of elements that satisfy a given condition.
    /// </summary>
    /// <param name="dbConnection">The database connections.</param>
    /// <typeparam name="TEntity">The type of the record.</typeparam>
    /// <returns>The total number of elements.</returns>
    public static int Count<TEntity>(this DbConnection dbConnection)
    {
        Type entity = typeof(TEntity);
        string table = entity.Name;
        string query = $"SELECT COUNT(1) FROM {table}s";

        return dbConnection.ExecuteScalar<int>(query);
    }
}
