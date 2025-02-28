namespace KISS.FluentSqlBuilder.QueryHandlerChain;

/// <summary>
///     SqlStatement.
/// </summary>
public enum SqlStatement
{
    Select,
    From,
    Join,
    Where,
    GroupBy,
    OrderBy,
    Limit,
    Offset
}
