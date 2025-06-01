namespace KISS.FluentSqlBuilder.QueryChain.OrderByHandlers;

/// <summary>
///     A handler for processing ORDER BY clauses in a query chain.
///     Provides functionality to sort query results by specified columns or expressions.
/// </summary>
/// <param name="Selector">
///     The expression selecting the columns or expressions to sort by.
///     Can represent single or multiple columns for ordering the result set.
/// </param>
public sealed partial record OrderByHandler(Expression Selector) : QueryHandler(SqlStatement.OrderBy, Selector)
{
    /// <inheritdoc />
    protected override void Process() => Composite = new OrderByDecorator(Composite);
}
