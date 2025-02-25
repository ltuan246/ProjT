namespace KISS.FluentSqlBuilder.QueryHandlerChain.QueryHandlers;

/// <summary>
///     GroupByHandler.
/// </summary>
/// <param name="Selector">Selector.</param>
public sealed record GroupByHandler(Expression Selector) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process() { }
}
