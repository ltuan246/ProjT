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
public sealed record LimitHandler(int Rows) : QueryHandler(SqlStatement.Limit)
{
    /// <inheritdoc />
    protected override void Process()
    {
        // Wraps the provided CompositeQuery with a LimitDecorator for LIMIT clause processing.
        Composite = new LimitDecorator(Composite);
        // Sets the LIMIT value in the SQL statement collection.
        Composite.SqlStatements[SqlStatement.Limit] = [$"{Rows}"];
    }
}
