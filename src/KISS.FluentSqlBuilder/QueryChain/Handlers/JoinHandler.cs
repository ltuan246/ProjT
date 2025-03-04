namespace KISS.FluentSqlBuilder.QueryChain.Handlers;

/// <summary>
///     A handler for processing join operations in a query chain, linking two relations via key equality.
/// </summary>
/// <typeparam name="TRelation">The type of the relation (source table or entity).</typeparam>
/// <param name="LeftKeySelector">An expression selecting the key from the left relation for the join condition (e.g., left => left.Id).</param>
/// <param name="RightKeySelector">An expression selecting the key from the right relation for the join condition (e.g., right => right.Id).</param>
/// <param name="MapSelector">An expression mapping the joined result into the output type (e.g., left => left.RightRelation).</param>
public abstract partial record JoinHandler<TRelation>(
    Expression LeftKeySelector,
    Expression RightKeySelector,
    Expression MapSelector) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
    {
        Append("INNER JOIN");
        AppendLine($"{typeof(TRelation).Name}s {Composite.GetAliasMapping(typeof(TRelation))}", true);
        AppendLine(" ON ", true);
        Translate(LeftKeySelector);
        Append(" = ");
        Translate(RightKeySelector);
    }
}
