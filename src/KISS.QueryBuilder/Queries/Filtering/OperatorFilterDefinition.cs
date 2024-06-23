namespace KISS.QueryBuilder.Queries.Filtering;

public sealed class OperatorFilterDefinition(
    ComparisonOperator comparisonOperator,
    RenderedFieldDefinition fieldDefinition,
    object value) : IOperatorFilterDefinition
{
    public (ComparisonOperator comparisonOperator, string fieldName, object value) QueryParameter { get; } =
        new(comparisonOperator, fieldDefinition.FieldName, value);

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);
}