namespace KISS.FluentQueryBuilder;

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
    public static FluentBuilder<TEntity> Retrieve<TEntity>(this DbConnection dbConnection) => new(dbConnection);
}
