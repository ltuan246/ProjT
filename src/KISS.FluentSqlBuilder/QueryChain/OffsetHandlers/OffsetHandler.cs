namespace KISS.FluentSqlBuilder.QueryChain.OffsetHandlers;

/// <summary>
///     A handler for processing OFFSET clauses in a query chain.
///     This class is responsible for generating SQL OFFSET statements that skip
///     a specified number of rows in the query results.
/// </summary>
/// <param name="Offset">
///     The number of rows to skip.
///     This value determines how many records to skip before starting to return results.
/// </param>
public sealed record OffsetHandler(int Offset) : QueryHandler
{
    /// <summary>
    ///     Processes the OFFSET clause by adding the row offset
    ///     to the query's OFFSET statements.
    /// </summary>
    protected override void Process()
        => Composite.SqlStatements[SqlStatement.Offset].Add($"{Offset}");
}
