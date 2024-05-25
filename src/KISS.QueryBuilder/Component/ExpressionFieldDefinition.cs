namespace KISS.QueryBuilder.Component;

public sealed record ExpressionFieldDefinition<TComponent, TField>(Expression<Func<TComponent, TField>> Expr) : FieldDefinition<TComponent, TField>
{
    public override RenderedFieldDefinition Render()
    {
        Expression ex = Expr.Body;
        var fieldName = ex.NodeType switch
        {
            ExpressionType.MemberAccess => ((MemberExpression)ex).Member.Name,
            _ => throw new NotSupportedException()
        };

        return new(fieldName);
    }
}