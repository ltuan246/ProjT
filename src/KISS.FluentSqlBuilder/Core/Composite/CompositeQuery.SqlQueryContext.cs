namespace KISS.FluentSqlBuilder.Core.Composite;

/// <summary>
///     A partial class that provides SQL query context and state management for the CompositeQuery class.
///     This class handles the construction and formatting of SQL queries, including parameter management
///     and statement organization.
/// </summary>
public sealed partial class CompositeQuery
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
    private StringBuilder SqlBuilder { get; } = new();

    /// <summary>
    ///     Gets the SQL formatter instance used for custom string formatting
    ///     and parameter handling in SQL queries.
    /// </summary>
    public SqlFormatter SqlFormatting { get; } = new();

    /// <summary>
    ///     Gets the dictionary that organizes SQL statements by their type.
    ///     This collection maintains separate lists for different SQL clauses
    ///     (SELECT, FROM, JOIN, etc.) to ensure proper query construction.
    /// </summary>
    public Dictionary<SqlStatement, List<string>> SqlStatements { get; } = new()
    {
        { SqlStatement.Select, [] },
        { SqlStatement.From, [] },
        { SqlStatement.Join, [] },
        { SqlStatement.Where, [] },
        { SqlStatement.GroupBy, [] },
        { SqlStatement.Having, [] },
        { SqlStatement.SelectAggregate, [] },
        { SqlStatement.OrderBy, [] },
        { SqlStatement.Limit, [] },
        { SqlStatement.Offset, [] }
    };

    /// <summary>
    ///     Gets the dictionary that maps table types to their SQL aliases.
    ///     This collection is used to maintain consistent table aliases
    ///     throughout the query construction process.
    /// </summary>
    private Dictionary<Type, string> TableAliases { get; } = [];

    /// <summary>
    ///     Gets the dictionary that maps grouping key names to their types.
    ///     This collection is used to maintain type information for
    ///     grouping operations in the query.
    /// </summary>
    public Dictionary<string, Type> GroupingKeys { get; } = [];

    /// <summary>
    ///     Gets the dictionary that maps aggregation key names to their types.
    ///     This collection is used to maintain type information for
    ///     aggregation operations in the query.
    /// </summary>
    public Dictionary<string, Type> AggregationKeys { get; } = [];
}
