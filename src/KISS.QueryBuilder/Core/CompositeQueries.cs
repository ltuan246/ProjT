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
        new() { [SingleItemAsArrayOperator.Contains] = " IN ", [SingleItemAsArrayOperator.NotContains] = " NOT IN " };

    private static Dictionary<LogicalOperator, string> LogicalOperators { get; } =
        new() { [LogicalOperator.And] = " AND ", [LogicalOperator.Or] = " OR " };

    private StringBuilder Builder { get; } = new();

    private static Dictionary<string, object> QueryParameters { get; } = new();

    private static int Position => QueryParameters.Count;

    private string Operation() => Builder.ToString();

    public static (string, Dictionary<string, object>) Render(IComponent expression)
    {
        CompositeQueries visitor = new();
        visitor.Visit(expression);
        return (visitor.Operation(), QueryParameters);
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
        (ComparisonOperator operatorName, ExpressionFieldDefinition<TComponent, TField> field, TField value) =
            operatorFilterDefinition;
        RenderedFieldDefinition renderedField = field.Render();
        string namedParameter = $"@p{Position}";
        Builder.Append($"{renderedField.FieldName}{FieldMatchingOperators[operatorName]}{namedParameter}");
        QueryParameters.Add(namedParameter, value!);
    }

    public void Visit<TComponent, TField>(
        SingleItemAsArrayOperatorFilterDefinition<TComponent, TField> operatorFilterDefinition)
    {
        (SingleItemAsArrayOperator operatorName, ExpressionFieldDefinition<TComponent, TField> field, TField[] values) =
            operatorFilterDefinition;
        RenderedFieldDefinition renderedField = field.Render();
        string[] namedParameters = values.Select((value, i) =>
        {
            string namedParameter = $"@p{i + Position}";
            QueryParameters.Add(namedParameter, value!);

            return namedParameter;
        }).ToArray();

        Builder.Append(
            $"{renderedField.FieldName}{SingleItemAsArrayOperators[operatorName]}({string.Join(',', namedParameters)})");
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