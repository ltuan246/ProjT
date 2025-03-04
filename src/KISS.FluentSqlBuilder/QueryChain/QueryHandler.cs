namespace KISS.FluentSqlBuilder.QueryChain;

/// <summary>
///     An abstract base record for handling SQL query construction in a chain-of-responsibility pattern.
///     Inherits from <see cref="ExpressionTranslator" /> to process query expressions
///     and delegates to the next handler if present.
/// </summary>
public abstract partial record QueryHandler : ExpressionTranslator
{
    /// <summary>
    ///     Gets or sets the next handler in the chain, enabling sequential processing of query components.
    ///     Initially null, set via <see cref="SetNext(QueryHandler)" /> to link handlers.
    /// </summary>
    private QueryHandler? NextHandler { get; set; }

    /// <summary>
    ///     Gets the <see cref="CompositeQuery" /> instance being processed by this handler.
    ///     Set during <see cref="Handle(CompositeQuery)" /> and accessible to derived classes for query manipulation.
    ///     Initialized as default! to suppress null warnings, assigned a valid instance before use.
    /// </summary>
    protected CompositeQuery Composite { get; private set; } = default!;

    /// <summary>
    ///     Links this handler to the next one in the chain, forming a sequence of query processing steps.
    /// </summary>
    /// <param name="nextHandler">The next <see cref="QueryHandler" /> to process the query after this one.</param>
    public void SetNext(QueryHandler nextHandler)
    {
        var lastHandler = this;
        while (lastHandler.NextHandler is not null)
        {
            lastHandler = lastHandler.NextHandler;
        }

        lastHandler.NextHandler = nextHandler;
    }

    /// <summary>
    ///     Processes the provided <see cref="CompositeQuery" /> by setting it as the current instance,
    ///     executing this handler's specific logic, and passing control to the next handler if available.
    /// </summary>
    /// <param name="composite">The <see cref="CompositeQuery" /> instance to be processed by the handler chain.</param>
    public void Handle(CompositeQuery composite)
    {
        // Assigns the provided CompositeQuery to this handler for processing.
        Composite = composite;
        // Invokes the derived class's specific processing logic for this handler.
        Process();
        BuildExpression();
        // Passes the CompositeQuery to the next handler in the chain, if one exists, to continue processing.
        NextHandler?.Handle(composite);
    }

    /// <summary>
    ///     Defines the specific processing logic for this handler, to be implemented by derived classes.
    ///     Typically translates expressions or modifies the <see cref="Composite" /> query state.
    /// </summary>
    protected abstract void Process();

    /// <summary>
    ///     Builds an expression processor to translate the handler's query component into executable SQL.
    ///     Derived classes can implement this method to define how their specific expressions
    ///     (e.g., join conditions, WHERE clauses) are processed and integrated into the <see cref="CompositeQuery"/>.
    /// </summary>
    protected virtual void BuildExpression() { }
}
