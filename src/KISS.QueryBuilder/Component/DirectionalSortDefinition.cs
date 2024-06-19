namespace KISS.QueryBuilder.Component;

public sealed class DirectionalSortDefinition(SortDirection sortDirection, RenderedFieldDefinition fieldDefinition)
    : ISortDefinition
{
    public (SortDirection sortDirection, string fieldName) OrderParameter { get; } =
        new(sortDirection, fieldDefinition.FieldName);

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}