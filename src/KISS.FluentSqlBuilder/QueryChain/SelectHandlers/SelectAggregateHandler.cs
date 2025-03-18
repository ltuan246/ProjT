namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     A handler for processing <c>SELECT</c> in a query chain.
/// </summary>
/// <param name="Selector">An expression defining the aggregation operation.</param>
/// <param name="Alias">The alias columns.</param>
public sealed partial record SelectAggregateHandler(Expression Selector, string Alias) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
    {
        Translate(Selector);
        Composite.SqlStatements[SqlStatement.SelectAggregate].Add($"{StatementBuilder}");
    }
}
