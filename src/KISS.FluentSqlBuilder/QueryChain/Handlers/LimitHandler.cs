namespace KISS.FluentSqlBuilder.QueryChain.Handlers;

/// <summary>
///     LimitHandler.
/// </summary>
/// <param name="Rows">Limit.</param>
public sealed record LimitHandler(int Rows) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
        => Composite.SqlStatements[SqlStatement.Limit].Add($"{Rows}");
}
