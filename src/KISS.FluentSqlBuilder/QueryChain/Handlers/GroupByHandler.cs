namespace KISS.FluentSqlBuilder.QueryChain.Handlers;

/// <summary>
///     GroupByHandler.
/// </summary>
/// <param name="Selector">Selector.</param>
public sealed partial record GroupByHandler(Expression Selector) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process() { }
}
