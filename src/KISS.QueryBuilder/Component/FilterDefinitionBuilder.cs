namespace KISS.QueryBuilder.Component;

public sealed record FilterDefinitionBuilder<TComponent>
{
    public ComparisonOperatorFilterDefinition<TComponent, TField> Eq<TField>(Expression<Func<TComponent, TField>> field,
        TField value)
        => new(ComparisonOperators.Equals, new(field), value);

    public LogicalOperatorFieldDefinition And(params IFilterDefinition[] filterDefinitions)
        => new(LogicalOperators.And, filterDefinitions);
}