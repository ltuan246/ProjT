namespace KISS.QueryBuilder.Component;

public sealed record OperatorFilterDefinition<TEntity, TField>(
    ComparisonOperator Operator,
    RenderedFieldDefinition Field,
    [DisallowNull] TField Value) : IFilterDefinition
{
    public (ComparisonOperator comparisonOperator, string fieldName, object value) QueryParameter { get; init; } =
        new(Operator, Field.FieldName, Value);

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}