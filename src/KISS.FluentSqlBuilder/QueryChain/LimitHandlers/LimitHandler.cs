namespace KISS.FluentSqlBuilder.QueryChain.LimitHandlers;

/// <summary>
///     A handler for processing <c>LIMIT</c> in a query chain.
/// </summary>
/// <param name="Rows">The number of rows to fetch.</param>
public sealed record LimitHandler(int Rows) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
        => Composite.SqlStatements[SqlStatement.Limit].Add($"{Rows}");
}
