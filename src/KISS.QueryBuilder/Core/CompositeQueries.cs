namespace KISS.QueryBuilder.Core;

public sealed class CompositeQueries : IVisitor
{
    private static IReadOnlyDictionary<ComparisonOperator, string> FieldMatchingOperators { get; } =
        new Dictionary<ComparisonOperator, string>
        {
            [ComparisonOperator.Equals] = " = ",
            [ComparisonOperator.NotEquals] = " <> ",
            [ComparisonOperator.Greater] = " > ",
            [ComparisonOperator.GreaterOrEquals] = " >= ",
            [ComparisonOperator.Less] = " < ",
            [ComparisonOperator.LessOrEquals] = " <= "
        };

    private static IReadOnlyDictionary<SingleItemAsArrayOperator, string> SingleItemAsArrayOperators { get; } =
        new Dictionary<SingleItemAsArrayOperator, string>
        {
            [SingleItemAsArrayOperator.Contains] = " IN ", [SingleItemAsArrayOperator.NotContains] = " NOT IN "
        };

    private static IReadOnlyDictionary<LogicalOperator, string> LogicalOperators { get; } =
        new Dictionary<LogicalOperator, string> { [LogicalOperator.And] = " AND ", [LogicalOperator.Or] = " OR " };

    private StringBuilder Builder { get; } = new();

    private static Dictionary<string, object> QueryParameters { get; } = [];

    private static int Position => QueryParameters.Count;

    private string Operation() => Builder.ToString();

    public static (string, Dictionary<string, object>) Render(IQuerying expression)
    {
        CompositeQueries visitor = new();
        visitor.Visit(expression);
        return (visitor.Operation(), QueryParameters);
    }

    private void Join(string separator, IEnumerable<IQuerying> expressions)
    {
        using IEnumerator<IQuerying> enumerator = expressions.GetEnumerator();
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

    public void Visit(IQuerying concreteQuerying) => concreteQuerying.Accept(this);

    public void Visit<TEntity, TField>(
        OperatorFilterDefinition<TEntity, TField> operatorFilterDefinition)
    {
        (ComparisonOperator operatorName, RenderedFieldDefinition field, TField value) =
            operatorFilterDefinition;

        Guard.Against.Null(value);

        string namedParameter = $"@p{Position}";
        QueryParameters.Add(namedParameter, value);

        string query = string.Join(' ',
            [field.FieldName, FieldMatchingOperators[operatorName], namedParameter]);

        Builder.Append(query);
    }

    public void Visit<TEntity, TField>(
        SingleItemAsArrayOperatorFilterDefinition<TEntity, TField> operatorFilterDefinition)
    {
        (SingleItemAsArrayOperator operatorName, ExpressionFieldDefinition<TEntity, TField> field, TField[] values) =
            operatorFilterDefinition;

        Guard.Against.NullOrEmpty(values);

        string[] namedParameters = values.Select((value, _) =>
        {
            Guard.Against.Null(value);

            string namedParameter = $"@p{Position}";
            QueryParameters.Add(namedParameter, value);

            return namedParameter;
        }).ToArray();

        RenderedFieldDefinition renderedField = field.Render();
        string query = string.Join(' ',
        [
            renderedField.FieldName, SingleItemAsArrayOperators[operatorName], $"({string.Join(',', namedParameters)})"
        ]);

        Builder.Append(query);
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