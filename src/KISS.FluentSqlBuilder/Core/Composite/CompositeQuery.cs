namespace KISS.FluentSqlBuilder.Core.Composite;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <param name="connection">The <see cref="DbConnection" /> used to execute the query.</param>
public sealed partial class CompositeQuery(DbConnection connection) : ICompositeQueryOperations
{
    /// <summary>
    ///     Gets the database connection used for executing SQL queries.
    ///     This connection is initialized via the constructor and remains constant
    ///     throughout the instance's lifetime, ensuring consistent database access.
    /// </summary>
    private DbConnection Connection { get; } = connection;

    /// <summary>
    ///     Executes the constructed SQL query against the database and returns the results
    ///     as a list of the specified type. This method handles both simple and complex
    ///     query scenarios, automatically selecting the appropriate processing strategy.
    /// </summary>
    /// <typeparam name="TReturn">
    ///     The type of objects to return, representing the query result rows.
    ///     This type must match the structure of the query results.
    /// </typeparam>
    /// <returns>
    ///     A list of <typeparamref name="TReturn" /> objects retrieved based on the query conditions.
    ///     The list is empty if no results are found.
    /// </returns>
    public List<TReturn> GetList<TReturn>()
    {
        System.Diagnostics.Debug.WriteLine(Sql);
        // Executes the SQL query using the Connection, passing the constructed Sql string and Parameters
        var dtRows = Connection.Query(Sql, Parameters)
            .Cast<IDictionary<string, object>>()
            .ToList();

        // Processes the raw data rows into a typed list of TReturn objects using a dynamic expression
        return JoinRowProcessors.Count switch
        {
            0 => SimpleProcessToList<TReturn>(dtRows),
            _ => UniqueProcessToList<TReturn>(dtRows)
        };
    }

    /// <summary>
    ///     Executes the constructed SQL query against the database and returns the results
    ///     as a dictionary with composite keys and lists of the specified type. This method
    ///     is designed for handling complex queries with multiple result sets or nested data.
    /// </summary>
    /// <typeparam name="TReturn">
    ///     The type of objects to return, representing the query result rows.
    ///     This type must match the structure of the query results.
    /// </typeparam>
    /// <returns>
    ///     A dictionary where the key is a composite tuple representing unique identifiers,
    ///     and the value is a list of <typeparamref name="TReturn" /> objects associated with
    ///     that key. The dictionary is empty if no results are found.
    /// </returns>
    public Dictionary<ITuple, List<TReturn>> GetDictionary<TReturn>()
    {
        System.Diagnostics.Debug.WriteLine(Sql);
        // Executes the SQL query using the Connection, passing the constructed Sql string and Parameters
        var dtRows = Connection.Query(Sql, Parameters)
            .Cast<IDictionary<string, object>>()
            .ToList();

        // Processes the raw data rows into a typed dictionary of TReturn objects using nested processing
        return NestedUniqueProcessToList<TReturn>(dtRows);
    }
}
