namespace KISS.QueryBuilder.Queries;

public sealed record ExpressionFieldDefinition<TEntity, TField>(Expression<Func<TEntity, TField>> Expr)
{
    public static implicit operator RenderedFieldDefinition(ExpressionFieldDefinition<TEntity, TField> field)
    {
        Expression ex = field.Expr.Body;
        string fieldName = ex.NodeType switch
        {
            ExpressionType.MemberAccess => $"[{((MemberExpression)ex).Member.Name}]",
            _ => throw new NotSupportedException()
        };

        return new(fieldName);
    }
}