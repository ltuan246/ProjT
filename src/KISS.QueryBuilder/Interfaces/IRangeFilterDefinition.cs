namespace KISS.QueryBuilder.Interfaces;

public interface IRangeFilterDefinition : IQuerying
{
    (string fieldName, object beginValue, object endValue) QueryParameter { get; }
}