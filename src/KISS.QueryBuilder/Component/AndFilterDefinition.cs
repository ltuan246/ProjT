namespace KISS.QueryBuilder.Component;

public sealed record AndFilterDefinition(params IQuerying[] FilterDefinitions) : IMultipleFiltersDefinition
{
    public (LogicalOperator logicalOperator, IQuerying[] filterDefinitions) GroupingFilterDefinition { get; } =
        new(LogicalOperator.And, FilterDefinitions);

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}