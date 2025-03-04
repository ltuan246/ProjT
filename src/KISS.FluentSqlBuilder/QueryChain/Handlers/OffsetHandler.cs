namespace KISS.FluentSqlBuilder.QueryChain.Handlers;

/// <summary>
///     OffsetHandler.
/// </summary>
/// <param name="Offset">Offset.</param>
public sealed record OffsetHandler(int Offset) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
        => Composite.SqlStatements[SqlStatement.Offset].Add($"{Offset}");
}
