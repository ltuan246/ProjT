namespace KISS.FluentSqlBuilder.Core;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TReturn">
///     The type of objects to return, representing the query result rows.
///     This type must match the structure of the query results.
/// </typeparam>
/// <param name="Connection">The <see cref="DbConnection" /> used to execute the query.</param>
public sealed partial record QueryOperator<TReturn>(CompositeQuery Composite, List<IDictionary<string, object>> InputData) : ICompositeQueryOperations<TReturn>
{
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
    public List<TReturn> GetList()
        => Composite.JoinRowProcessors.Count switch
        {
            0 => SimpleProcessToList(),
            _ => UniqueProcessToList()
        };

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
    public Dictionary<ITuple, List<TReturn>> GetDictionary()
        => NestedUniqueProcessToList();
}
