namespace KISS.FluentSqlBuilder.Core;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">
///     The type of objects to return, representing the query result rows.
///     This type must match the structure of the query results.
/// </typeparam>
public sealed partial record QueryOperator<TRecordset, TReturn>
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
    {
        // Compiles the expression tree
        var lambda = Expression
            .Lambda<Func<List<IDictionary<string, object>>, List<TReturn>>>(
                Composite.Block,
                Composite.InEntriesExParameter)
            .Compile();

        // Executes the expression tree, returning the result
        return lambda(InputData);
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
    public Dictionary<ITuple, List<TReturn>> GetDictionary()
    {
        // Compiles the expression tree
        var lambda = Expression
            .Lambda<Func<List<IDictionary<string, object>>, Dictionary<ITuple, List<TReturn>>>>(
                Composite.Block,
                Composite.InEntriesExParameter)
            .Compile();

        // Executes the expression tree, returning the result
        return lambda(InputData);
    }
}
