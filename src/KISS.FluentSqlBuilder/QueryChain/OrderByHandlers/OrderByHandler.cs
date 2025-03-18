namespace KISS.FluentSqlBuilder.QueryChain.OrderByHandlers;

/// <summary>
///     A handler for processing <c>ORDER BY</c> in a query chain.
/// </summary>
/// <param name="Selector">The table columns.</param>
public sealed partial record OrderByHandler(Expression Selector) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
    {
        Translate(Selector);
        Composite.SqlStatements[SqlStatement.OrderBy].Add($"{StatementBuilder}");
    }
}
