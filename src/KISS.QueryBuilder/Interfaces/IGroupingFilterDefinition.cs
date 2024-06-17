namespace KISS.QueryBuilder.Interfaces;

public interface IGroupingFilterDefinition : IQuerying
{
    (LogicalOperator logicalOperator, IQuerying[] filterDefinitions) GroupingFilterDefinition { get; }
}