namespace KISS.FluentSqlBuilder.QueryChain.GroupByHandlers;

/// <summary>
///     A handler for processing <c>GROUP BY</c> in a query chain.
/// </summary>
/// <param name="Selector">An additional key to further refine the grouping.</param>
public sealed partial record GroupByHandler(Expression Selector) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
    {
        Translate(Selector);
        Composite.SqlStatements[SqlStatement.GroupBy].Add($"{StatementBuilder}");
    }
}
