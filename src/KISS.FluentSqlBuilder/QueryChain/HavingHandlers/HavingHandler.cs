namespace KISS.FluentSqlBuilder.QueryChain.HavingHandlers;

/// <summary>
///     A handler for processing <c>HAVING</c> in a query chain.
/// </summary>
/// <param name="Predicate">Filters a sequence of values based on a predicate.</param>
public sealed record HavingHandler(Expression Predicate) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process() { }
}
