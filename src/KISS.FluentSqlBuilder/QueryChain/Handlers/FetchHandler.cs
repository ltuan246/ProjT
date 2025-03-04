namespace KISS.FluentSqlBuilder.QueryChain.Handlers;

/// <summary>
///     FetchHandler.
/// </summary>
/// <param name="Offset">Offset.</param>
public sealed record FetchHandler(Expression Offset) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process() { }
}
