namespace KISS.QueryBuilder.Component;

public sealed class SingleItemAsArrayOperatorFilterDefinition<TField>(
    SingleItemAsArrayOperator comparisonOperator,
    RenderedFieldDefinition fieldDefinition,
    params TField[] values) : ISingleItemAsArrayOperatorFilterDefinition
{
    public (SingleItemAsArrayOperator singleItemAsArrayOperator, string fieldName, object[] values)
        QueryParameter { get; } = new(comparisonOperator, fieldDefinition.FieldName, values.Cast<object>().ToArray());

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}