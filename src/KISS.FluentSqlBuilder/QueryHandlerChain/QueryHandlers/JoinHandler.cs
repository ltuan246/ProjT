namespace KISS.FluentSqlBuilder.QueryHandlerChain.QueryHandlers;

/// <summary>
/// A handler for joining recordsets with a relation, allowing key-based joins and mapping results.
/// </summary>
/// <param name="LeftKeySelector">An expression to select the key from the primary recordset.</param>
/// <param name="RightKeySelector">An expression to select the key from the related entity.</param>
/// <param name="MapSelector">An expression to map the resulting relation to the return type.</param>
public sealed record JoinHandler(
    Expression LeftKeySelector,
    Expression RightKeySelector,
    Expression MapSelector) : QueryHandler
{
    /// <inheritdoc />
    public override void Handle(CompositeQuery context)
    {
        _ = context;
        base.Handle(context);
    }
}
