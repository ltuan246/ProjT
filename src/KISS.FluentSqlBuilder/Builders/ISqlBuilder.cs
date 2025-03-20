namespace KISS.FluentSqlBuilder.Builders;

/// <summary>
///     Defines the base interface for SQL query builders. This interface serves as the
///     foundation for all SQL query building operations in the library.
/// </summary>
public interface ISqlBuilder;

/// <summary>
///     Defines an interface for building and executing SQL queries that operate on a single table.
///     This interface provides methods for executing queries and retrieving results.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface ISqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Executes the SQL query and returns the results as a list of strongly-typed objects.
    ///     This method performs the actual database query and maps the results to the specified return type.
    /// </summary>
    /// <returns>
    ///     A list of objects of type TReturn containing the query results. Each object
    ///     represents a row from the query result, mapped according to the query's
    ///     column selection and projection.
    /// </returns>
    List<TReturn> ToList();
}

/// <summary>
///     Defines an interface for building and executing SQL queries that join two tables.
///     This interface provides methods for executing queries and retrieving results from
///     queries involving two tables.
/// </summary>
/// <typeparam name="TFirst">The type of the first table in the query.</typeparam>
/// <typeparam name="TSecond">The type of the second table in the query.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface ISqlBuilder<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Executes the SQL query and returns the results as a list of strongly-typed objects.
    ///     This method performs the actual database query and maps the results from both tables
    ///     to the specified return type.
    /// </summary>
    /// <returns>
    ///     A list of objects of type TReturn containing the query results. Each object
    ///     represents a row from the joined query result, mapped according to the query's
    ///     column selection and projection.
    /// </returns>
    List<TReturn> ToList();
}

/// <summary>
///     Defines an interface for building and executing SQL queries that join three tables.
///     This interface provides methods for executing queries and retrieving results from
///     complex queries involving three tables.
/// </summary>
/// <typeparam name="TFirst">The type of the first table in the query.</typeparam>
/// <typeparam name="TSecond">The type of the second table in the query.</typeparam>
/// <typeparam name="TThird">The type of the third table in the query.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface ISqlBuilder<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Executes the SQL query and returns the results as a list of strongly-typed objects.
    ///     This method performs the actual database query and maps the results from all three
    ///     tables to the specified return type.
    /// </summary>
    /// <returns>
    ///     A list of objects of type TReturn containing the query results. Each object
    ///     represents a row from the joined query result, mapped according to the query's
    ///     column selection and projection.
    /// </returns>
    List<TReturn> ToList();
}

/// <summary>
///     Defines an interface for building and executing SQL queries that include GROUP BY
///     clauses and return results as a dictionary. This interface is specifically designed
///     for queries that group data and return aggregated results.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface IGroupSqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Executes the SQL query and returns the results as a dictionary where the key
    ///     represents the grouping values and the value is a list of results for that group.
    /// </summary>
    /// <returns>
    ///     A dictionary where:
    ///     <list type="bullet">
    ///         <item>The key is an ITuple containing the grouping values</item>
    ///         <item>The value is a list of TReturn objects for that group</item>
    ///     </list>
    /// </returns>
    Dictionary<ITuple, List<TReturn>> ToDictionary();
}
