namespace KISS.FluentSqlBuilder.QueryChain.OrderByHandlers;

/// <summary>
///     A handler for processing <c>ORDER BY</c> clauses in a query chain.
///     Provides functionality to sort query results by specified columns.
/// </summary>
/// <param name="Selector">The expression selecting the columns to sort by.</param>
public sealed partial record OrderByHandler(Expression Selector) : QueryHandler
{
    /// <summary>
    ///     Processes the ORDER BY clause and adds it to the query.
    /// </summary>
    protected override void Process()
    {
        Translate(Selector);
        Composite.SqlStatements[SqlStatement.OrderBy].Add($"{StatementBuilder}");
    }
}
