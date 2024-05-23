namespace KISS.QueryBuilder.Component;

public sealed record ExpressionFieldDefinition<TComponent, TField>(Expression<Func<TComponent, TField>> Expr)
{
    public string FieldName
    {
        get
        {
            Expression ex = Expr.Body;
            return ex.NodeType switch
            {
                ExpressionType.MemberAccess => ((MemberExpression)ex).Member.Name,
                _ => throw new NotSupportedException()
            };
        }
    }
}