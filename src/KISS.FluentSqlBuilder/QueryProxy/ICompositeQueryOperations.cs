namespace KISS.FluentSqlBuilder.QueryProxy;

/// <summary>
///     Defines the contract for data retrieval operations in the SQL query builder.
///     This interface provides methods for executing SQL queries and retrieving results
///     in different formats, with built-in query setup and execution logic.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">
///     The type of objects to return, representing the query result rows.
///     This type must match the structure of the query results.
/// </typeparam>
public interface ICompositeQueryOperations<TRecordset, TReturn>
{
    /// <summary>
    ///     Executes the constructed SQL query and returns the results as a list of the specified type.
    /// </summary>
    /// <typeparam name="TReturn">The type of objects to return, representing the query result rows.</typeparam>
    /// <returns>
    ///     A list of <typeparamref name="TReturn" /> objects containing the query results.
    ///     The results are automatically mapped from the database columns to the specified type.
    /// </returns>
    List<TReturn> GetList();

    /// <summary>
    ///     Executes the constructed SQL query and returns the results as a dictionary,
    ///     where the key is a tuple of grouping keys and the value is a list of results.
    /// </summary>
    /// <typeparam name="TReturn">The type of objects to return, representing the query result rows.</typeparam>
    /// <returns>
    ///     A dictionary where the key is an <see cref="ITuple" /> containing the grouping keys,
    ///     and the value is a list of <typeparamref name="TReturn" /> objects for that group.
    /// </returns>
    Dictionary<ITuple, List<TReturn>> GetDictionary();
}
