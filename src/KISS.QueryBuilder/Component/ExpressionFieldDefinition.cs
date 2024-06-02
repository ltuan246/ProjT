namespace KISS.QueryBuilder.Component;

public sealed record ExpressionFieldDefinition<TComponent, TField>(Expression<Func<TComponent, TField>> Expr)
{
    public RenderedFieldDefinition Render()
    {
        Expression ex = Expr.Body;
        string fieldName = ex.NodeType switch
        {
            ExpressionType.MemberAccess => ((MemberExpression)ex).Member.Name,
            _ => throw new NotSupportedException()
        };

        return new(fieldName);
    }
}