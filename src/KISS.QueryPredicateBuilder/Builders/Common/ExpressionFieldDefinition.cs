namespace KISS.QueryPredicateBuilder.Builders.Common;

/// <summary>
/// The Expression helper containing frequently reused functions.
/// </summary>
/// <param name="Expression">The MemberExpression.</param>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TField"></typeparam>
public sealed record ExpressionFieldDefinition<TEntity, TField>(Expression<Func<TEntity, TField>> Expression)
{
    public static implicit operator string(
        [NotNull] ExpressionFieldDefinition<TEntity, TField> expressionFieldDefinition)
        => new(expressionFieldDefinition.GetMemberNameFromLambda());

    private string GetMemberNameFromLambda()
    {
        var body = Expression.Body;
        MemberExpression memberExpression = body.NodeType switch
        {
            ExpressionType.MemberAccess => (MemberExpression)body,
            _ => throw new NotSupportedException(),
        };
        var memberInfo = memberExpression.Member;
        return memberInfo.Name;
    }
}
