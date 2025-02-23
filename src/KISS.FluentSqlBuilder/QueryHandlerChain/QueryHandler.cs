namespace KISS.FluentSqlBuilder.QueryHandlerChain;

/// <summary>
///     QueryHandler.
/// </summary>
public abstract record QueryHandler
{
    /// <summary>
    ///     NextHandler.
    /// </summary>
    public QueryHandler? NextHandler { get; set; }

    /// <summary>
    ///     SetNext.
    /// </summary>
    /// <param name="nextHandler">NextHandler.</param>
    public void SetNext(QueryHandler nextHandler) => NextHandler = nextHandler;

    /// <summary>
    ///     Handle.
    /// </summary>
    /// <param name="context">CompositeQuery.</param>
    public virtual void Handle(CompositeQuery context) => NextHandler?.Handle(context); // Pass to the next handler
}
