namespace KISS.FluentSqlBuilder.QueryChain.JoinHandlers;

/// <summary>
///     A handler for processing one-to-many join operations in a query chain.
///     This class is responsible for generating SQL JOIN statements and mapping
///     joined results to strongly-typed objects with one-to-many relationships.
/// </summary>
/// <typeparam name="TRecordset">
///     The type representing the database record set.
///     This is typically the main entity type being queried.
/// </typeparam>
/// <typeparam name="TRelation">
///     The type of the relation (source table or entity).
///     This type is used to generate proper table names and column mappings.
/// </typeparam>
/// <typeparam name="TReturn">
///     The combined type to return.
///     This type represents the result of the join operation.
/// </typeparam>
/// <param name="LeftKeySelector">
///     An expression selecting the key from the left relation for the join condition.
///     This defines the left side of the join equality.
/// </param>
/// <param name="RightKeySelector">
///     An expression selecting the key from the right relation for the join condition.
///     This defines the right side of the join equality.
/// </param>
/// <param name="MapSelector">
///     An expression mapping the joined result into the output type.
///     This defines how the joined data should be mapped to the result object.
/// </param>
public sealed record OneToManyJoinHandler<TRecordset, TRelation, TReturn>(
    Expression LeftKeySelector,
    Expression RightKeySelector,
    Expression MapSelector) : JoinHandler<TRelation>(LeftKeySelector, RightKeySelector, MapSelector)
{
    /// <summary>
    ///     Builds the expression tree for processing joined rows.
    ///     This method creates the necessary expressions to map joined data
    ///     to collections in the result object's properties.
    /// </summary>
    protected override void ExpressionIntegration()
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
