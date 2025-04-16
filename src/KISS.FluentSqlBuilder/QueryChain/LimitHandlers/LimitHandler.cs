namespace KISS.FluentSqlBuilder.QueryChain.LimitHandlers;

/// <summary>
///     A handler for processing LIMIT clauses in a query chain.
///     This class is responsible for generating SQL LIMIT statements that restrict
///     the number of rows returned by a query.
/// </summary>
/// <param name="Rows">
///     The number of rows to fetch.
///     This value determines the maximum number of records returned by the query.
/// </param>
public sealed record LimitHandler(int Rows) : QueryHandler(Expression.Block(), SqlStatement.Limit)
{
    /// <summary>
    ///     Processes the LIMIT clause by adding the row limit
    ///     to the query's LIMIT statements.
    /// </summary>
    protected override void TranslateExpression()
        => Composite.SqlStatements[SqlStatement.Limit].Add($"{Rows}");
}
