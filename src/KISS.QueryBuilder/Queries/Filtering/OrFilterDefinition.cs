namespace KISS.QueryBuilder.Queries.Filtering;

public sealed class OrFilterDefinition(params IQuerying[] filters) : IMultipleFiltersDefinition
{
    public (LogicalOperator logicalOperator, IQuerying[] filterDefinitions) GroupingFilterDefinition { get; } =
        new(LogicalOperator.Or, filters);

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);
}