namespace KISS.FluentSqlBuilder.QueryChain.FetchHandlers;

/// <summary>
///     A handler for processing <c>FETCH</c> in a query chain.
/// </summary>
/// <param name="Offset">The number of rows to skip.</param>
public sealed record FetchHandler(Expression Offset) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process() { }
}
