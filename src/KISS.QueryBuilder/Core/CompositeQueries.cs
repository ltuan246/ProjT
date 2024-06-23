namespace KISS.QueryBuilder.Core;

public sealed class CompositeQueries<TEntity> : IVisitor
{
    private static Type Entity => typeof(TEntity);

    private static IEnumerable<PropertyInfo> Properties => Entity.GetProperties();

    private List<string> Columns { get; } = [];

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

    private Dictionary<string, object> QueryParameters { get; } = [];

    private string NamedParameterMarkers => $"@p{QueryParameters.Count}";

    public static (string, Dictionary<string, object>) Render(IQuerying expression)
    {
        CompositeQueries<TEntity> visitor = new();
        visitor.Visit(expression);
        return (visitor.Builder.ToString(), visitor.QueryParameters);
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

    public void Visit(IBuilder builder)
    {
        PushState(QueryingContext.Composite, builder.Queries.Count());
        Join(string.Empty, builder.Queries);
        PopState();
    }

    public void Visit(IOperatorFilterDefinition operatorFilterDefinition)
    {
        (ComparisonOperator operatorName, string fieldName, object value) =
            operatorFilterDefinition.QueryParameter;

        Guard.Against.Null(value);

        string namedParameter = NamedParameterMarkers;
        QueryParameters.Add(namedParameter, value);

        string query = string.Join(' ',
            [fieldName, QueryBuildHelper.FieldMatchingOperators[operatorName], namedParameter]);

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
            fieldName,
            QueryBuildHelper.SingleItemAsArrayOperators[singleItemAsArrayOperator],
            $"({string.Join(',', namedParameters)})"
        ]);

        if (Context != QueryingContext.MultipleFilters ||
            (Context == QueryingContext.MultipleFilters && StackStatePosition == 0))
        {
            query = $" WHERE {query}";
        }

        Builder.Append(query);
    }

    public void Visit(IRangeFilterDefinition rangeFilterDefinition)
    {
        (string fieldName, object beginValue, object endValue) =
            rangeFilterDefinition.QueryParameter;

        Guard.Against.Null(beginValue);
        Guard.Against.Null(endValue);

        string beginNamedParameter = NamedParameterMarkers;
        QueryParameters.Add(beginNamedParameter, beginValue);

        string endNamedParameter = NamedParameterMarkers;
        QueryParameters.Add(endNamedParameter, endValue);

        const string betweenOperator = "({0} BETWEEN {1} AND {2})";
        StringBuilder betweenOperatorBuilder = new();
        betweenOperatorBuilder.AppendFormat(betweenOperator, fieldName, beginNamedParameter, endNamedParameter);
        string query = betweenOperatorBuilder.ToString();

        if (Context != QueryingContext.MultipleFilters ||
            (Context == QueryingContext.MultipleFilters && StackStatePosition == 0))
        {
            query = $" WHERE {query}";
        }

        Builder.Append(query);
    }

    public void Visit(ICombinedFilterDefinition filters)
    {
        (LogicalOperator logicalOperator, IQuerying[] filterDefinitions) =
            filters.GroupingFilterDefinition;

        PushState(QueryingContext.MultipleFilters, filterDefinitions.Length);
        Join(QueryBuildHelper.LogicalOperators[logicalOperator], filterDefinitions);
        PopState();
    }

    public void Visit(ISortDefinition filterDefinition)
    {
        (SortDirection sortDirection, string fieldName) =
            filterDefinition.OrderParameter;

        Guard.Against.Null(fieldName);

        string query = string.Join(' ',
            [fieldName, QueryBuildHelper.OrderByOperators[sortDirection]]);

        if (Context != QueryingContext.MultipleSorts ||
            (Context == QueryingContext.MultipleSorts && StackStatePosition == 0))
        {
            query = $" ORDER BY {query}";
        }

        Builder.Append(query);
    }

    public void Visit(ICombinedSortDefinition sorts)
    {
        PushState(QueryingContext.MultipleSorts, sorts.Sorts.Count());
        Join(", ", sorts.Sorts);
        PopState();
    }

    public void Visit(ISingleFieldProjectionDefinition singleFieldProjection)
    {
        (RenderedFieldDefinition field, bool isIncluding) =
            singleFieldProjection.FieldDefinition;

        if (isIncluding)
        {
            Columns.Add(field.FieldName);
        }
        else
        {
            if (!Columns.Any())
            {
                Columns.AddRange(Properties.Select(p => p.Name).ToArray());
            }

            Columns.Remove(field.FieldName);
        }
    }

    public void Visit(ISliceProjectionDefinition sliceProjectionDefinition)
    {
        if (Context != QueryingContext.CombinedProjection ||
            (Context == QueryingContext.CombinedProjection && StackStatePosition == 0))
        {
            string query = $" SELECT TOP {sliceProjectionDefinition.Limit} ";
            Builder.Append(query);
        }
    }

    public void Visit(ICombinedProjectionDefinition combinedProjectionDefinition)
    {
        PushState(QueryingContext.CombinedProjection, combinedProjectionDefinition.Projections.Length);
        Join(string.Empty, combinedProjectionDefinition.Projections);
        PopState();
    }
}