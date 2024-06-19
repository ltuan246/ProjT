namespace KISS.QueryBuilder.Interfaces;

public interface ISortDefinition : IQuerying
{
    (SortDirection sortDirection, string fieldName) OrderParameter { get; }
}