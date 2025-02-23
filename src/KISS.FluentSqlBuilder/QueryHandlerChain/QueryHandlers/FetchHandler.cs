namespace KISS.FluentSqlBuilder.QueryHandlerChain.QueryHandlers;

/// <summary>
///     FetchHandler.
/// </summary>
/// <param name="Offset">Offset.</param>
public sealed record FetchHandler(Expression Offset) : QueryHandler;
