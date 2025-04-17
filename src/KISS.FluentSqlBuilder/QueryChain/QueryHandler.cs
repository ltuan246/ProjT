namespace KISS.FluentSqlBuilder.QueryChain;

/// <summary>
///     An abstract base record for handling SQL query construction in a chain-of-responsibility pattern.
///     This class serves as the foundation for all query component handlers, providing a structured
///     approach to processing different parts of a SQL query (SELECT, WHERE, JOIN, etc.).
/// </summary>
public abstract partial record QueryHandler(SqlStatement Statement, Expression? Selector = null) : ExpressionTranslator
{
    /// <summary>
    ///     Gets or sets the next handler in the chain, enabling sequential processing of query components.
    ///     This property implements the chain-of-responsibility pattern by allowing handlers to be linked
    ///     together in a sequence for processing different parts of a query.
    /// </summary>
    private QueryHandler? NextHandler { get; set; }

    /// <summary>
    ///     Gets the CompositeQuery instance being processed by this handler.
    ///     This property provides access to the query being built and allows handlers
    ///     to modify the query state during processing.
    /// </summary>
    protected CompositeQuery Composite { get; private set; } = default!;

    /// <summary>
    ///     Links this handler to the next one in the chain, forming a sequence of query processing steps.
    ///     This method implements the chain-of-responsibility pattern by finding the last handler
    ///     in the current chain and appending the new handler.
    /// </summary>
    /// <param name="nextHandler">The next QueryHandler to process the query after this one.</param>
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
    ///     Processes the provided CompositeQuery by setting it as the current instance,
    ///     executing this handler's specific logic, and passing control to the next handler if available.
    ///     This method implements the core processing logic of the chain-of-responsibility pattern.
    /// </summary>
    /// <param name="composite">The CompositeQuery instance to be processed by the handler chain.</param>
    public void Handle(CompositeQuery composite)
    {
        // Assigns the provided CompositeQuery to this handler for processing.
        Composite = composite;
        // Invokes the derived class's specific processing logic for this handler.
        Process();
        TranslateExpression();
        ExpressionIntegration();
        // Passes the CompositeQuery to the next handler in the chain, if one exists, to continue processing.
        NextHandler?.Handle(composite);
    }

    /// <summary>
    ///     Defines the specific processing logic for this handler, to be implemented by derived classes.
    ///     This abstract method allows each handler to define its own processing logic for specific
    ///     query components (SELECT, WHERE, JOIN, etc.).
    /// </summary>
    protected virtual void Process() { }

    /// <summary>
    ///     Translates the stored expression (Selector) into a SQL statement fragment
    ///     and appends it to the Composite's SQL statements collection for the specified statement type.
    /// </summary>
    protected virtual void TranslateExpression()
    {
        Translate(Selector);

        if (!string.IsNullOrEmpty(Sql))
        {
            Composite.SqlStatements[Statement].Add(Sql);
        }
    }

    /// <summary>
    ///     Builds an expression processor to translate the handler's query component into executable SQL.
    ///     This virtual method provides a hook for handlers to define how their specific expressions
    ///     are processed and integrated into the final SQL query.
    /// </summary>
    protected virtual void ExpressionIntegration() { }
}
