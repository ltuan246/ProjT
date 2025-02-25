namespace KISS.FluentSqlBuilder.QueryHandlerChain.QueryHandlers;

/// <summary>
///     LimitHandler.
/// </summary>
/// <param name="Rows">Limit.</param>
public sealed record LimitHandler(int Rows) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process() { }
}
