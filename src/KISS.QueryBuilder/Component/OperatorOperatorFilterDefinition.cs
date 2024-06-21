namespace KISS.QueryBuilder.Component;

public sealed class OperatorOperatorFilterDefinition(
    ComparisonOperator comparisonOperator,
    RenderedFieldDefinition fieldDefinition,
    object value) : IOperatorFilterDefinition
{
    public (ComparisonOperator comparisonOperator, string fieldName, object value) QueryParameter { get; } =
        new(comparisonOperator, fieldDefinition.FieldName, value);

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}