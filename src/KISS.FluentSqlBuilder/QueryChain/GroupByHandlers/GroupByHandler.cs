namespace KISS.FluentSqlBuilder.QueryChain.GroupByHandlers;

/// <summary>
///     Handles the processing of GROUP BY clauses within a query chain.
///     This class generates SQL GROUP BY statements to group query results
///     based on specified columns or expressions provided by the selector.
/// </summary>
/// <param name="Selector">
///     An expression specifying the columns or expressions to group by.
///     Can represent a single column, multiple columns, or complex grouping logic.
/// </param>
public sealed partial record GroupByHandler(Expression Selector) : QueryHandler(SqlStatement.GroupBy, Selector)
{
    /// <inheritdoc />
    protected override void Process() =>
        // Wraps the current composite query with a GroupByDecorator for GROUP BY processing.
        Composite = new GroupByDecorator(Composite);
}
