namespace KISS.FluentSqlBuilder.QueryChain.Handlers;

/// <summary>
///     A handler for processing join operations in a query chain, linking two relations via key equality.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TRelation">The type of the relation (source table or entity).</typeparam>
/// <param name="LeftKeySelector">An expression selecting the key from the left relation for the join condition (e.g., left => left.Id).</param>
/// <param name="RightKeySelector">An expression selecting the key from the right relation for the join condition (e.g., right => right.Id).</param>
/// <param name="MapSelector">An expression mapping the joined result into the output type (e.g., left => left.RightRelation).</param>
public sealed record OneToOneJoinHandler<TRecordset, TRelation>(
    Expression LeftKeySelector,
    Expression RightKeySelector,
    Expression MapSelector) : JoinHandler<TRelation>(LeftKeySelector, RightKeySelector, MapSelector)
{
    /// <inheritdoc />
    protected override void BuildExpression()
    {
        if (MapSelector is MemberExpression { Expression: ParameterExpression } memberExpression)
        {
            Expression Init((ParameterExpression IterRowParameter, IndexExpression CurrentEntityVariable) p)
            {
                return Expression.Block(
                    Expression.Assign(
                        Expression.Property(p.CurrentEntityVariable, memberExpression.Member.Name),
                        Expression.MemberInit(
                            Expression.New(typeof(TRelation)),
                            Composite.CreateIterRowBindings(p.IterRowParameter, typeof(TRecordset), typeof(TRelation)))));
            }

            Composite.JoinRowProcessors.Add(Init);
        }
    }
}
