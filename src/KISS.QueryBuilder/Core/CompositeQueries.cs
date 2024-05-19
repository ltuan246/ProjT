namespace KISS.QueryBuilder.Core;

public sealed class CompositeQueries : Visitor
{
    private static Dictionary<ComparisonOperators, string> FieldMatchingOperators { get; } =
        new()
        {
            [ComparisonOperators.Equals] = " = ",
            [ComparisonOperators.NotEquals] = " <> ",
            [ComparisonOperators.Greater] = " > ",
            [ComparisonOperators.GreaterOrEquals] = " >= ",
            [ComparisonOperators.Less] = " < ",
            [ComparisonOperators.LessOrEquals] = " <= ",
        };

    private static Dictionary<LogicalOperators, string> LogicalOperators { get; } =
        new()
        {
            [Enums.LogicalOperators.And] = " AND ",
            [Enums.LogicalOperators.Or] = " OR "
        };

    private StringBuilder Builder { get; } = new();

    public string Render() => Builder.ToString();

    public static string Render(IComponent expression)
    {
        CompositeQueries visitor = new();
        visitor.Visit(expression);
        return visitor.Render();
    }

    private void Join(IEnumerable<IComponent> expressions)
    {
        using var enumerator = expressions.GetEnumerator();
        while (enumerator.MoveNext())
        {
            enumerator.Current.Accept(this);
        }
    }

    public override void Visit<TComponent, TField>(ComparisonOperatorFilterDefinition<TComponent, TField> operatorFilterDefinition)
    {
        var (operatorName, field, value) = operatorFilterDefinition;
        Builder.Append($"{field.FieldName}{FieldMatchingOperators[operatorName]}{value}");
    }

    public override void Visit(LogicalOperatorFieldDefinition logicalOperatorFieldDefinition)
    {
        var filterDefinitions = logicalOperatorFieldDefinition.FilterDefinitions;
        Join(filterDefinitions);
    }
}