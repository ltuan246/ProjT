namespace KISS.QueryBuilder;

/// <summary>
///     An extension class for database connections.
/// </summary>
public static class DbConnectionExtensions
{
    /// <summary>
    ///     Retrieves data from a database based on conditions.
    /// </summary>
    /// <param name="dbConnection">The database connections.</param>
    /// <typeparam name="TEntity">The type of the record.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    public static FluentSqlBuilder<TEntity> Retrieving<TEntity>(this DbConnection dbConnection)
    {
        _ = dbConnection;
        return new FluentSqlBuilder<TEntity>();
    }
}
