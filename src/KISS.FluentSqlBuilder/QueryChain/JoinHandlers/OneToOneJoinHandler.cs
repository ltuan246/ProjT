namespace KISS.FluentSqlBuilder.QueryChain.JoinHandlers;

/// <summary>
///     A handler for processing one-to-one join operations in a query chain.
///     This class generates SQL JOIN statements and maps the joined result to a single related entity
///     property on the output type, supporting strongly-typed one-to-one relationships.
/// </summary>
/// <typeparam name="TRecordset">
///     The type representing the database record set (main entity).
/// </typeparam>
/// <typeparam name="TRelation">
///     The type of the related entity (joined table or entity).
/// </typeparam>
/// <typeparam name="TReturn">
///     The combined type to return as the result of the join operation.
/// </typeparam>
/// <param name="LeftKeySelector">
///     An expression selecting the key from the left relation for the join condition (e.g., left => left.Id).
/// </param>
/// <param name="RightKeySelector">
///     An expression selecting the key from the right relation for the join condition (e.g., right => right.Id).
/// </param>
/// <param name="MapSelector">
///     An expression mapping the joined result into the output type (e.g., left => left.RightRelation).
/// </param>
public sealed record OneToOneJoinHandler<TRecordset, TRelation, TReturn>(
    Expression LeftKeySelector,
    Expression RightKeySelector,
    Expression MapSelector) : JoinHandler<TRelation, TReturn>(LeftKeySelector, RightKeySelector)
{
    /// <summary>
    ///     Integrates the join mapping logic into the expression tree for processing joined rows.
    ///     This method creates the necessary expressions to map the related entity into
    ///     a property of the result object, handling initialization and assignment.
    /// </summary>
    protected override void ExpressionIntegration()
    {
        if (MapSelector is MemberExpression { Expression: ParameterExpression } memberExpression)
        {
            var init = Expression.Block(
                Expression.Assign(
                    Expression.Property(Composite.IndexerExVariable, memberExpression.Member.Name),
                    Expression.MemberInit(
                        Expression.New(RelationType),
                        TypeUtils.CreateIterRowBindings(
                            Composite.CurrentEntryExVariable,
                            RelationType,
                            RelationType,
                            Composite.GetAliasMapping(RelationType)))));

            Composite.JoinRows.Add(init);
        }
    }
}
