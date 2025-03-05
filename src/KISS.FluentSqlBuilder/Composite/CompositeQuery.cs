namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     Implements <see cref="ICompositeQueryOperations" /> to provide query execution capabilities.
/// </summary>
/// <param name="connection">The <see cref="DbConnection" /> used to execute the query.</param>
public sealed partial class CompositeQuery(DbConnection connection) : ICompositeQueryOperations
{
    /// <summary>
    ///     Gets the database connection used for executing SQL queries.
    ///     Initialized via the constructor and remains constant throughout the instance's lifetime.
    /// </summary>
    private DbConnection Connection { get; } = connection;

    /// <summary>
    ///     Executes the constructed SQL query against the database and returns the results as a list of the specified type.
    /// </summary>
    /// <typeparam name="TReturn">The type of objects to return, representing the query result rows.</typeparam>
    /// <returns>A list of <typeparamref name="TReturn" /> objects retrieved based on the query conditions.</returns>
    public List<TReturn> GetList<TReturn>()
    {
        // Executes the SQL query using the Connection, passing the constructed Sql string and Parameters (presumed properties).
        // Casts the result to a list of dictionaries for flexible row data access.
        var dtRows = Connection.Query(Sql, Parameters)
            .Cast<IDictionary<string, object>>()
            .ToList();

        // Processes the raw data rows into a typed list of TReturn objects using a dynamic expression.
        return JoinRowProcessors.Count switch
        {
            0 => SimpleProcessToList<TReturn>(dtRows),
            _ => UniqueProcessToList<TReturn>(dtRows)
        };
    }

    /// <summary>
    ///     Executes the constructed SQL query against the database and returns the results as a list of the specified type.
    /// </summary>
    /// <typeparam name="TReturn">The type of objects to return, representing the query result rows.</typeparam>
    /// <returns>A list of <typeparamref name="TReturn" /> objects retrieved based on the query conditions.</returns>
    public Dictionary<ITuple, List<TReturn>> GetDictionary<TReturn>()
    {
        System.Diagnostics.Debug.WriteLine(Sql);
        // Executes the SQL query using the Connection, passing the constructed Sql string and Parameters (presumed properties).
        // Casts the result to a list of dictionaries for flexible row data access.
        var dtRows = Connection.Query(Sql, Parameters)
            .Cast<IDictionary<string, object>>()
            .ToList();

        // Processes the raw data rows into a typed list of TReturn objects using a dynamic expression.
        return NestedUniqueProcessToList<TReturn>(dtRows);
    }
}
