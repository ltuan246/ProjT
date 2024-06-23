namespace KISS.QueryBuilder.Interfaces;

public interface ICombinedFilterDefinition : IQuerying
{
    (LogicalOperator logicalOperator, IQuerying[] filterDefinitions) GroupingFilterDefinition { get; }
}