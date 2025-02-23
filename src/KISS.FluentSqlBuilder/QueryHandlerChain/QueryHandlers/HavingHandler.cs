namespace KISS.FluentSqlBuilder.QueryHandlerChain.QueryHandlers;

/// <summary>
///     HavingHandler.
/// </summary>
/// <param name="Predicate">Predicate.</param>
public sealed record HavingHandler(Expression Predicate) : QueryHandler;
