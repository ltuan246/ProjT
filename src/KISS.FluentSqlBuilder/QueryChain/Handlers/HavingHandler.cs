namespace KISS.FluentSqlBuilder.QueryChain.Handlers;

/// <summary>
///     HavingHandler.
/// </summary>
/// <param name="Predicate">Predicate.</param>
public sealed record HavingHandler(Expression Predicate) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process() { }
}
