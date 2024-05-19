namespace KISS.QueryBuilder.Component;

public sealed record FieldDefinition<TComponent, TField>(Expression<Func<TComponent, TField>> expression)
{
    public Expression<Func<TComponent, TField>> Expression { get; } = expression;

    public string FieldName
    {
        get { return ""; }
    }

}