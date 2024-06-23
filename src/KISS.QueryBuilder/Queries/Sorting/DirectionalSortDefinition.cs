namespace KISS.QueryBuilder.Queries.Sorting;

public sealed class DirectionalSortDefinition(SortDirection sortDirection, RenderedFieldDefinition fieldDefinition)
    : ISortDefinition
{
    public (SortDirection sortDirection, string fieldName) OrderParameter { get; } =
        new(sortDirection, fieldDefinition.FieldName);

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);
}