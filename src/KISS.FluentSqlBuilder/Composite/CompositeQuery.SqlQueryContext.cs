namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     CompositeQuery.
/// </summary>
public sealed partial class CompositeQuery
{
    /// <summary>
    ///     Gets the generated the SQL.
    /// </summary>
    private string Sql
        => SqlBuilder.ToString();

    /// <summary>
    ///     A collection of dynamic parameters that can be used in SQL queries.
    /// </summary>
    private DynamicParameters Parameters
        => SqlFormatting.Parameters;

    /// <summary>
    ///     Sets the generated the SQL.
    /// </summary>
    public StringBuilder SqlBuilder { get; } = new();

    /// <summary>
    ///     Use to custom string formatting for SQL queries.
    /// </summary>
    public SqlFormatter SqlFormatting { get; } = new();

    /// <summary>
    ///     Sets the generated the SQL.
    /// </summary>
    public Dictionary<SqlStatement, List<FormattableString>> SqlStatements { get; } = new()
    {
        { SqlStatement.Select, [] },
        { SqlStatement.From, [] },
        { SqlStatement.Join, [] },
        { SqlStatement.Where, [] },
        { SqlStatement.GroupBy, [] },
        { SqlStatement.OrderBy, [] },
        { SqlStatement.Limit, [] },
        { SqlStatement.Offset, [] }
    };

    /// <summary>
    ///     A collection specifically for table aliases.
    /// </summary>
    public Dictionary<Type, string> TableAliases { get; } = [];

    /// <summary>
    ///     A collection specifically for grouping keys.
    /// </summary>
    public Dictionary<string, Type> GroupingKeys { get; } = [];
}
