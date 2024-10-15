namespace KISS.QueryPredicateBuilder;

/// <summary>
///     An extension class for database connections.
/// </summary>
public static class DbConnectionExtensions
{
    /// <summary>
    ///     Retrieves data from a database based on conditions.
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

        var entity = typeof(TEntity);
        var propsName = entity.GetProperties().Select(p => p.Name).ToArray();
        var columns = string.Join(", ", propsName);
        var table = entity.Name;
        var query = $"SELECT {columns} FROM {table}s";

        return dbConnection.Query<TEntity>(query).ToList();
    }

    /// <summary>
    ///     Returns the total number of elements in a collection or the number of elements that satisfy a given condition.
    /// </summary>
    /// <param name="dbConnection">The database connections.</param>
    /// <typeparam name="TEntity">The type of the record.</typeparam>
    /// <returns>The total number of elements.</returns>
    public static int Count<TEntity>(this DbConnection dbConnection)
    {
        var entity = typeof(TEntity);
        var table = entity.Name;
        var query = $"SELECT COUNT(1) FROM {table}s";

        return dbConnection.ExecuteScalar<int>(query);
    }
}
