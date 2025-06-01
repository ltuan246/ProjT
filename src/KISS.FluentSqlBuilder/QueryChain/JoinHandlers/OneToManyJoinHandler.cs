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
///     The type of the related entity (joined table or collection).
///     Used to generate table names, column mappings, and collection properties.
/// </typeparam>
/// <typeparam name="TReturn">
///     The result type produced by the join operation, representing the combined output.
/// </typeparam>
/// <param name="LeftKeySelector">
///     An expression selecting the key from the left (main) entity for the join condition.
///     Defines the left side of the join equality.
/// </param>
/// <param name="RightKeySelector">
///     An expression selecting the key from the right (related) entity for the join condition.
///     Defines the right side of the join equality.
/// </param>
/// <param name="MapSelector">
///     An expression mapping the joined result into the output type,
///     specifying how the related entities are assigned to the result object.
/// </param>
public sealed record OneToManyJoinHandler<TRecordset, TRelation, TReturn>(
    Expression LeftKeySelector,
    Expression RightKeySelector,
    Expression MapSelector) : JoinHandler<TRelation, TReturn>(LeftKeySelector, RightKeySelector)
{
    /// <summary>
    ///     Integrates the join mapping logic into the expression tree for processing joined rows.
    ///     This method creates the necessary expressions to map related entities into
    ///     collection properties of the result object, handling initialization and population.
    /// </summary>
    protected override void ExpressionIntegration()
    {
        if (MapSelector is MemberExpression { Expression: ParameterExpression } memberExpression)
        {
            // Access or initialize the relation property (e.g., List<TRelation>) on the result object.
            var relationProperty = Expression.Property(Composite.IndexerExVariable, memberExpression.Member.Name);
            var nullCheck = Expression.Equal(relationProperty, Expression.Constant(null));
            var assignList = Expression.Assign(relationProperty, Expression.New(relationProperty.Type));

            // Create a new TRelation instance from the current row.
            var relationEntity = Expression.MemberInit(
                Expression.New(RelationType),
                TypeUtils.CreateIterRowBindings(
                    Composite.CurrentEntryExVariable,
                    RelationType,
                    RelationType,
                    Composite.GetAliasMapping(RelationType)));

            // If the collection is null, initialize it; then always add the new entity.
            var init = Expression.Block(
                Expression.IfThen(nullCheck, assignList), // Only assign if null
                Expression.Call(
                    relationProperty,
                    relationProperty.Type.GetMethod("Add")!,
                    relationEntity)); // Always add afterward

            Composite.JoinRows.Add(init);
        }
    }
}
