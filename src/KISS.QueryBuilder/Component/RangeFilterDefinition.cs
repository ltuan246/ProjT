namespace KISS.QueryBuilder.Component;

public sealed class RangeFilterDefinition(
    RenderedFieldDefinition fieldDefinition,
    object beginValue,
    object endValue) : IRangeFilterDefinition
{
    public (string fieldName, object beginValue, object endValue) QueryParameter { get; } =
        new(fieldDefinition.FieldName, beginValue, endValue);

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}