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
            [ComparisonOperators.LessOrEquals] = " <= "
        };

    private static Dictionary<SingleItemAsArrayOperators, string> ItemAsArrayOperators { get; } =
        new()
        {
            [SingleItemAsArrayOperators.Contains] = " IN ",
            [SingleItemAsArrayOperators.NotContains] = " NOT IN "
        };

    private static Dictionary<LogicalOperators, string> LogicalOperators { get; } =
        new() { [Enums.LogicalOperators.And] = " AND ", [Enums.LogicalOperators.Or] = " OR " };

    private StringBuilder Builder { get; } = new();

    private string Operation() => Builder.ToString();

    public static string Render(IComponent expression)
    {
        CompositeQueries visitor = new();
        visitor.Visit(expression);
        return visitor.Operation();
    }

    private void Join(string separator, IEnumerable<IComponent> expressions)
    {
        using IEnumerator<IComponent> enumerator = expressions.GetEnumerator();
        if (enumerator.MoveNext())
        {
            enumerator.Current.Accept(this);
            while (enumerator.MoveNext())
            {
                Builder.Append(separator);
                enumerator.Current.Accept(this);
            }
        }
    }

    public override void Visit<TComponent, TField>(
        ComparisonOperatorFilterDefinition<TComponent, TField> operatorFilterDefinition)
    {
        (ComparisonOperators operatorName, FieldDefinition<TComponent, TField> field, TField value) =
            operatorFilterDefinition;
        Builder.Append($"{field.FieldName}{FieldMatchingOperators[operatorName]}{value}");
    }

    public override void Visit<TComponent, TField>(
        SingleItemAsArrayOperatorFilterDefinition<TComponent, TField> operatorFilterDefinition)
    {
        (SingleItemAsArrayOperators operatorName, FieldDefinition<TComponent, TField> field, TField[] value) =
            operatorFilterDefinition;
        Builder.Append($"{field.FieldName}{ItemAsArrayOperators[operatorName]}({string.Join(',', value)})");
    }

    public override void Visit(LogicalOperatorFieldDefinition logicalOperatorFieldDefinition)
    {
        (LogicalOperators operatorName, IEnumerable<IFilterDefinition> filters) =
            logicalOperatorFieldDefinition;
        Join(LogicalOperators[operatorName], filters);
    }
}