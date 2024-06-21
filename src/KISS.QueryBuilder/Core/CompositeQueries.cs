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

    private static IReadOnlyDictionary<SortDirection, string> OrderByOperators { get; } =
        new Dictionary<SortDirection, string>
        {
            [SortDirection.Ascending] = " ASC ", [SortDirection.Descending] = " DESC "
        };

    private StringBuilder Builder { get; } = new();

    private Stack<StatesBuilder> StackStates { get; } = new();

    private void PushState(QueryingContext context, int length = 1)
    {
        StatesBuilder newState = new() { Context = context, Position = 0, Length = length };
        StackStates.Push(newState);
    }

    private void PopState()
    {
        StackStates.Pop();
    }

    private QueryingContext Context => StackStates.Peek().Context;

    private int StackStatePosition
    {
        get => StackStates.Peek().Position;
        set => StackStates.Peek().Position = value;
    }

    private static Dictionary<string, object> QueryParameters { get; } = [];

    private static string NamedParameterMarkers => $"@p{QueryParameters.Count}";

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
                ++StackStatePosition;
                Builder.Append(separator);
                enumerator.Current.Accept(this);
            }
        }
    }

    public void Visit(IQuerying concreteQuerying) => concreteQuerying.Accept(this);

    public void Visit(IQueryBuilders queryBuilders)
    {
        PushState(QueryingContext.Composite, queryBuilders.Queries.Count());
        Join(string.Empty, queryBuilders.Queries);
        PopState();
    }

    public void Visit(IFilterDefinition filterDefinition)
    {
        (ComparisonOperator operatorName, string fieldName, object value) =
            filterDefinition.QueryParameter;

        Guard.Against.Null(value);

        string namedParameter = NamedParameterMarkers;
        QueryParameters.Add(namedParameter, value);

        string query = string.Join(' ',
            [fieldName, FieldMatchingOperators[operatorName], namedParameter]);

        if (Context != QueryingContext.MultipleFilters ||
            (Context == QueryingContext.MultipleFilters && StackStatePosition == 0))
        {
            query = $" WHERE {query}";
        }

        Builder.Append(query);
    }

    public void Visit(ISingleItemAsArrayOperatorFilterDefinition operatorFilterDefinition)
    {
        (SingleItemAsArrayOperator singleItemAsArrayOperator, string fieldName, object[] values) =
            operatorFilterDefinition.QueryParameter;

        Guard.Against.NullOrEmpty(values);

        string[] namedParameters = values.Select((value, _) =>
        {
            Guard.Against.Null(value);

            string namedParameter = NamedParameterMarkers;
            QueryParameters.Add(namedParameter, value);

            return namedParameter;
        }).ToArray();

        string query = string.Join(' ',
        [
            fieldName, SingleItemAsArrayOperators[singleItemAsArrayOperator], $"({string.Join(',', namedParameters)})"
        ]);

        if (Context != QueryingContext.MultipleFilters ||
            (Context == QueryingContext.MultipleFilters && StackStatePosition == 0))
        {
            query = $" WHERE {query}";
        }

        Builder.Append(query);
    }

    public void Visit(IMultipleFiltersDefinition filters)
    {
        (LogicalOperator logicalOperator, IQuerying[] filterDefinitions) =
            filters.GroupingFilterDefinition;

        PushState(QueryingContext.MultipleFilters, filterDefinitions.Length);
        Join(LogicalOperators[logicalOperator], filterDefinitions);
        PopState();
    }

    public void Visit(ISortDefinition filterDefinition)
    {
        (SortDirection sortDirection, string fieldName) =
            filterDefinition.OrderParameter;

        Guard.Against.Null(fieldName);

        string query = string.Join(' ',
            [fieldName, OrderByOperators[sortDirection]]);

        if (Context != QueryingContext.MultipleSorts ||
            (Context == QueryingContext.MultipleSorts && StackStatePosition == 0))
        {
            query = $" ORDER BY {query}";
        }

        Builder.Append(query);
    }

    public void Visit(IMultipleSortsDefinition sorts)
    {
        PushState(QueryingContext.MultipleSorts, sorts.Sorts.Count());
        Join(", ", sorts.Sorts);
        PopState();
    }

    private sealed class StatesBuilder
    {
        public required QueryingContext Context { get; init; }

        public required int Position { get; set; }

        public required int Length { get; init; }
    }
}