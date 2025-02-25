namespace KISS.FluentSqlBuilder.QueryHandlerChain.QueryHandlers;

/// <summary>
///     WhereHandler.
/// </summary>
/// <param name="Predicate">Predicate.</param>
public sealed record WhereHandler(Expression Predicate) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process() { }
}
