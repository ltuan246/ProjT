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
    /// Composite.
    /// </summary>
    protected CompositeQuery Composite { get; private set; } = default!;

    /// <summary>
    ///     SetNext.
    /// </summary>
    /// <param name="nextHandler">NextHandler.</param>
    public void SetNext(QueryHandler nextHandler) => NextHandler = nextHandler;

    /// <summary>
    ///     Handle.
    /// </summary>
    /// <param name="composite">CompositeQuery.</param>
    public void Handle(CompositeQuery composite)
    {
        Composite = composite;
        Process();
        NextHandler?.Handle(composite);
    }

    /// <summary>
    /// Process.
    /// </summary>
    protected abstract void Process();
}
