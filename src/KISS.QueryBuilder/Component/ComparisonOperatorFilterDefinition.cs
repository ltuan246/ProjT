namespace KISS.QueryBuilder.Component;

public sealed record ComparisonOperatorFilterDefinition<TComponent, TField>(ComparisonOperators operatorName, FieldDefinition<TComponent, TField> field, TField value) : IComponent
{
    public ComparisonOperators Operator { get; } = operatorName;
    public FieldDefinition<TComponent, TField> Field { get; } = field;
    public TField Value { get; } = value;

    public void Deconstruct(out ComparisonOperators operatorName, out FieldDefinition<TComponent, TField> field, out TField value)
    {
        operatorName = Operator;
        field = Field;
        value = Value;
    }

    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);

    public string Render() => CompositeQueries.Render(this);
}