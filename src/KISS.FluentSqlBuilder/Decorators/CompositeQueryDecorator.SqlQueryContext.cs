namespace KISS.FluentSqlBuilder.Decorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TIn">The type representing the database record set.</typeparam>
/// <typeparam name="TOut">The combined type to return.</typeparam>
public sealed partial record CompositeQueryDecorator
{
    /// <summary>
    ///     Gets the final SQL query string generated from the query builder.
    ///     This property combines all SQL statements in the correct order and applies
    ///     any necessary formatting.
    /// </summary>
    public string Sql
        => SqlBuilder.ToString();

    /// <summary>
    ///     Gets the collection of dynamic parameters used in the SQL query.
    ///     These parameters are used for safe parameter binding and preventing SQL injection.
    /// </summary>
    public DynamicParameters Parameters
        => SqlFormatting.Parameters;

    /// <summary>
    ///     Gets the StringBuilder instance used to construct the SQL query.
    ///     This builder accumulates SQL statements and clauses during query construction.
    /// </summary>
    public StringBuilder SqlBuilder { get; } = InnerComposite.SqlBuilder;

    /// <summary>
    ///     Gets the SQL formatter instance used for custom string formatting
    ///     and parameter handling in SQL queries.
    /// </summary>
    public SqlFormatter SqlFormatting { get; } = InnerComposite.SqlFormatting;

    /// <summary>
    ///     Gets the dictionary that organizes SQL statements by their type.
    ///     This collection maintains separate lists for different SQL clauses
    ///     (SELECT, FROM, JOIN, etc.) to ensure proper query construction.
    /// </summary>
    public Dictionary<SqlStatement, List<string>> SqlStatements { get; } = InnerComposite.SqlStatements;

    /// <summary>
    ///     Gets the dictionary that maps table types to their SQL aliases.
    ///     This collection is used to maintain consistent table aliases
    ///     throughout the query construction process.
    /// </summary>
    public Dictionary<Type, string> TableAliases { get; } = InnerComposite.TableAliases;

    /// <summary>
    ///     Gets the dictionary that maps grouping key names to their types.
    ///     This collection is used to maintain type information for
    ///     grouping operations in the query.
    /// </summary>
    public Dictionary<string, Type> GroupingKeys { get; } = InnerComposite.GroupingKeys;

    /// <summary>
    ///     Gets the dictionary that maps aggregation key names to their types.
    ///     This collection is used to maintain type information for
    ///     aggregation operations in the query.
    /// </summary>
    public Dictionary<string, Type> AggregationKeys { get; } = InnerComposite.AggregationKeys;
}
