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
public sealed partial record GroupByHandler(Expression Selector) : QueryHandler
{
    /// <summary>
    ///     Processes the GROUP BY clause by translating the selector expression
    ///     into SQL and adding it to the query's GROUP BY statements.
    /// </summary>
    protected override void Process()
    {
        Translate(Selector);
        Composite.SqlStatements[SqlStatement.GroupBy].Add($"{StatementBuilder}");
    }
}
