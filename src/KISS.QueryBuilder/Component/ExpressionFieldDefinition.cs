namespace KISS.QueryBuilder.Component;

public sealed record ExpressionFieldDefinition<TEntity, TField>(Expression<Func<TEntity, TField>> Expr)
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

    public static implicit operator RenderedFieldDefinition(ExpressionFieldDefinition<TEntity, TField> field)
    {
        Expression ex = field.Expr.Body;
        string fieldName = ex.NodeType switch
        {
            ExpressionType.MemberAccess => ((MemberExpression)ex).Member.Name,
            _ => throw new NotSupportedException()
        };

        return new(fieldName);
    }
}

public sealed record ExpressionFieldDefinition<TEntity>(LambdaExpression Expression) : FieldDefinition<TEntity>
{
}