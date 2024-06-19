namespace KISS.QueryBuilder.Interfaces;

public interface IMultipleFiltersDefinition : IQuerying
{
    (LogicalOperator logicalOperator, IQuerying[] filterDefinitions) GroupingFilterDefinition { get; }
}