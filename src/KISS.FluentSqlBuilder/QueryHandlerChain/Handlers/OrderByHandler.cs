namespace KISS.FluentSqlBuilder.QueryHandlerChain.Handlers;

/// <summary>
///     OrderByHandler.
/// </summary>
/// <param name="Selector">Selector.</param>
public sealed record OrderByHandler(Expression Selector) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process() { }
}
