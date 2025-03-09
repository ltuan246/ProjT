namespace KISS.FluentSqlBuilder.QueryChain.OffsetHandlers;

/// <summary>
///     A handler for processing <c>OFFSET</c> in a query chain.
/// </summary>
/// <param name="Offset">The number of rows to skip.</param>
public sealed record OffsetHandler(int Offset) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
        => Composite.SqlStatements[SqlStatement.Offset].Add($"{Offset}");
}
