namespace KISS.FluentSqlBuilder.QueryChain.GroupByHandlers;

/// <summary>
///     A handler for processing GROUP BY clauses in a query chain.
///     This class is responsible for generating SQL GROUP BY statements that group
///     query results based on specified columns or expressions.
/// </summary>
/// <param name="Selector">
///     An expression defining the columns or expressions to group by.
///     This can be a single column, multiple columns, or complex expressions.
/// </param>
public sealed partial record GroupByHandler(Expression Selector) : QueryHandler(SqlStatement.GroupBy, Selector)
{
    /// <inheritdoc />
    protected override void Process() =>
        // Assigns the provided CompositeQuery to this handler for processing.
        Composite = new GroupByDecorator(Composite);
}
