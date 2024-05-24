namespace KISS.QueryBuilder.Core;

public sealed class CompositeQueries : IVisitor
{
    private static Dictionary<ComparisonOperator, string> FieldMatchingOperators { get; } =
        new()
        {
            [ComparisonOperator.Equals] = " = ",
            [ComparisonOperator.NotEquals] = " <> ",
            [ComparisonOperator.Greater] = " > ",
            [ComparisonOperator.GreaterOrEquals] = " >= ",
            [ComparisonOperator.Less] = " < ",
            [ComparisonOperator.LessOrEquals] = " <= "
        };

    private static Dictionary<SingleItemAsArrayOperator, string> SingleItemAsArrayOperators { get; } =
        new()
        {
            [SingleItemAsArrayOperator.Contains] = " IN ",
            [SingleItemAsArrayOperator.NotContains] = " NOT IN "
        };

    private static Dictionary<LogicalOperator, string> LogicalOperators { get; } =
        new()
        {
            [LogicalOperator.And] = " AND ",
            [LogicalOperator.Or] = " OR "
        };

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

    public void Visit(IComponent concreteComponent) => concreteComponent.Accept(this);

    public void Visit<TComponent, TField>(
        OperatorFilterDefinition<TComponent, TField> operatorFilterDefinition)
    {
        (ComparisonOperator operatorName, FieldDefinition<TComponent, TField> field, TField value) =
            operatorFilterDefinition;
        var renderedField = field.Render();
        Builder.Append($"{renderedField.FieldName}{FieldMatchingOperators[operatorName]}{value}");
    }

    public void Visit<TComponent, TField>(
        SingleItemAsArrayOperatorFilterDefinition<TComponent, TField> operatorFilterDefinition)
    {
        (SingleItemAsArrayOperator operatorName, ExpressionFieldDefinition<TComponent, TField> field, TField[] value) =
            operatorFilterDefinition;
        var renderedField = field.Render();
        Builder.Append($"{renderedField.FieldName}{SingleItemAsArrayOperators[operatorName]}({string.Join(',', value)})");
    }

    public void Visit(AndFilterDefinition filterDefinition)
    {
        Join(LogicalOperators[LogicalOperator.And], filterDefinition.FilterDefinitions);
    }

    public void Visit(OrFilterDefinition filterDefinition)
    {
        Join(LogicalOperators[LogicalOperator.Or], filterDefinition.FilterDefinitions);
    }
}