namespace KISS.QueryBuilder.Queries.Filtering;

public sealed class CombinedFilterDefinition(LogicalOperator logicalOperator, IQuerying[] filters)
    : ICombinedFilterDefinition
{
    public (LogicalOperator logicalOperator, IQuerying[] filterDefinitions) GroupingFilterDefinition { get; } =
        new(logicalOperator, filters);

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);
}