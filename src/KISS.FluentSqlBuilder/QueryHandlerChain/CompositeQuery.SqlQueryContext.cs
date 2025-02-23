namespace KISS.FluentSqlBuilder.QueryHandlerChain;

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
        => SqlFormat.Parameters;

    /// <summary>
    ///     Sets the generated the SQL.
    /// </summary>
    private StringBuilder SqlBuilder { get; } = new();

    /// <summary>
    ///     Use to custom string formatting for SQL queries.
    /// </summary>
    private SqlFormatter SqlFormat { get; } = new();

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    private bool HasOpenParentheses { get; set; }
}
