namespace KISS.FluentSqlBuilder.QueryHandlerChain.QueryHandlers;

/// <summary>
///     OffsetHandler.
/// </summary>
/// <param name="Offset">Offset.</param>
public sealed record OffsetHandler(int Offset) : QueryHandler;
