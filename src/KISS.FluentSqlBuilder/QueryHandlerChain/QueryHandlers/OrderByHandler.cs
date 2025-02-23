namespace KISS.FluentSqlBuilder.QueryHandlerChain.QueryHandlers;

/// <summary>
///     OrderByHandler.
/// </summary>
/// <param name="Selector">Selector.</param>
public sealed record OrderByHandler(Expression Selector) : QueryHandler;
