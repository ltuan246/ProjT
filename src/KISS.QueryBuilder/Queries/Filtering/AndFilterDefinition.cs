namespace KISS.QueryBuilder.Queries.Filtering;

public sealed class AndFilterDefinition(params IQuerying[] filters) : IMultipleFiltersDefinition
{
    public (LogicalOperator logicalOperator, IQuerying[] filterDefinitions) GroupingFilterDefinition { get; } =
        new(LogicalOperator.And, filters);

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);
}