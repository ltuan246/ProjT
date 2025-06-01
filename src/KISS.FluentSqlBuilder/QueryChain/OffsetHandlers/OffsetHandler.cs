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
public sealed record OffsetHandler(int Offset) : QueryHandler(SqlStatement.Offset)
{
    /// <inheritdoc />
    protected override void Process()
    {
        // Assigns the provided CompositeQuery to this handler for processing.
        Composite = new OffsetDecorator(Composite);
        Composite.SqlStatements[SqlStatement.Offset] = [$"{Offset}"];
    }
}
