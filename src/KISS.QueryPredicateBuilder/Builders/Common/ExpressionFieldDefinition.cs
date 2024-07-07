namespace KISS.QueryPredicateBuilder.Builders.Common;

public sealed record ExpressionFieldDefinition<TEntity, TField>(Expression<Func<TEntity, TField>> Expression)
{
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

    public static implicit operator string(ExpressionFieldDefinition<TEntity, TField> expressionFieldDefinition)
        => new(expressionFieldDefinition.GetMemberNameFromLambda());
}