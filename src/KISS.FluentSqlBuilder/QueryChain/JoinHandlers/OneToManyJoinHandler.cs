namespace KISS.FluentSqlBuilder.QueryChain.JoinHandlers;

/// <summary>
///     A handler for processing join operations in a query chain, linking two relations via key equality.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TRelation">The type of the relation (source table or entity).</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
/// <param name="LeftKeySelector">An expression selecting the key from the left relation for the join condition (e.g., left => left.Id).</param>
/// <param name="RightKeySelector">An expression selecting the key from the right relation for the join condition (e.g., right => right.Id).</param>
/// <param name="MapSelector">An expression mapping the joined result into the output type (e.g., left => left.RightRelation).</param>
public sealed record OneToManyJoinHandler<TRecordset, TRelation, TReturn>(
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
                // Access or initialize the Relation property (List<TRelation>)
                var relationProperty = Expression.Property(p.CurrentEntityVariable, memberExpression.Member.Name);
                var listType = typeof(List<TRelation>);
                var nullCheck = Expression.Equal(relationProperty, Expression.Constant(null));
                var assignList = Expression.Assign(relationProperty, Expression.New(listType));

                // Create a new TRelation instance
                var relationEntity = Expression.MemberInit(
                    Expression.New(typeof(TRelation)),
                    Composite.CreateIterRowBindings(p.IterRowParameter, typeof(TRelation), typeof(TRelation)));

                // If null, initialize; then add unconditionally
                return Expression.Block(
                    Expression.IfThen(nullCheck, assignList), // Only assign if null
                    Expression.Call(
                        relationProperty,
                        listType.GetMethod("Add")!,
                        relationEntity)); // Always add afterward
            }

            Composite.JoinRowProcessors.Add(Init);
        }
    }
}
