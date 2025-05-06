namespace KISS.FluentSqlBuilder.QueryChain.JoinHandlers;

/// <summary>
///     A handler for processing one-to-one join operations in a query chain.
///     Links two relations via key equality and maps the result to a single entity.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TRelation">The type of the relation (source table or entity).</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
/// <param name="LeftKeySelector">An expression selecting the key from the left relation for the join condition (e.g., left => left.Id).</param>
/// <param name="RightKeySelector">An expression selecting the key from the right relation for the join condition (e.g., right => right.Id).</param>
/// <param name="MapSelector">An expression mapping the joined result into the output type (e.g., left => left.RightRelation).</param>
public sealed record OneToOneJoinHandler<TRecordset, TRelation, TReturn>(
    Expression LeftKeySelector,
    Expression RightKeySelector,
    Expression MapSelector) : JoinHandler<TRelation, TReturn>(LeftKeySelector, RightKeySelector)
{
    /// <summary>
    ///     Builds the expression for mapping joined results to the output type.
    /// </summary>
    protected override void ExpressionIntegration()
    {
        if (MapSelector is MemberExpression { Expression: ParameterExpression } memberExpression)
        {
            Composite.OutDictEntityTypeExVariable ??= Expression.Variable(OutDictEntityType, "OutDictEntityTypeExVariable");
            Composite.OutDictKeyExVariable ??= Expression.Variable(OutDictKeyType, "OutDictKeyExVariable");

            var outKeyAccessor = Expression.MakeIndex(
                Composite.OutDictEntityTypeExVariable!,
                OutDictEntityType.GetProperty("Item")!,
                [OutDictKeyExVariable]);

            var init = Expression.Block(
                   Expression.Assign(
                       Expression.Property(outKeyAccessor, memberExpression.Member.Name),
                       Expression.MemberInit(
                           Expression.New(RelationType),
                           Composite.CreateIterRowBindings(Composite.CurrentEntryExParameter, RelationType, RelationType))));

            Composite.JoinRowProcessors.Add(init);
            JoinRowBlock = init;
        }
    }
}
