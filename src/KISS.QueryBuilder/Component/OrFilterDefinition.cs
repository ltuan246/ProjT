namespace KISS.QueryBuilder.Component;

public sealed record OrFilterDefinition(params IQuerying[] FilterDefinitions) : IGroupingFilterDefinition
{
    public (LogicalOperator logicalOperator, IQuerying[] filterDefinitions) GroupingFilterDefinition { get; } =
        new(LogicalOperator.Or, FilterDefinitions);

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}