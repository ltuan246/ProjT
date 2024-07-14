namespace KISS.QueryBuilder.Core;

public sealed class CompositeQueries<TEntity> : IVisitor
{
    private static Type Entity => typeof(TEntity);

    private static IEnumerable<PropertyInfo> Properties => Entity.GetProperties();

    private List<string> Columns { get; } = [];

    private StringBuilder Builder { get; } = new();

    private ConcurrentDictionary<QueryClause, StringBuilder> DicBuilder { get; } = new();

    private Dictionary<string, object> QueryParameters { get; } = [];

    private string NamedParameterMarkers => $"@p{QueryParameters.Count}";

    public static (string, Dictionary<string, object>) Render(IQuerying expression)
    {
        CompositeQueries<TEntity> visitor = new();
        visitor.Visit(expression);
        return (visitor.Builder.ToString(), visitor.QueryParameters);
    }

    private void Join(QueryClause clause, string separator, IEnumerable<IQuerying> expressions)
    {
        using IEnumerator<IQuerying> enumerator = expressions.GetEnumerator();
        if (enumerator.MoveNext())
        {
            enumerator.Current.Accept(this);
            while (enumerator.MoveNext())
            {
                if (clause != QueryClause.Default && DicBuilder.TryGetValue(clause, out StringBuilder? value))
                {
                    value.Append(separator);
                }

                enumerator.Current.Accept(this);
            }
        }
    }

    public void Visit(IQuerying concreteQuerying) => concreteQuerying.Accept(this);

    public void Visit(IBuilder builder)
    {
        Join(QueryClause.Default, string.Empty, builder.Queries);
        QueryClause[] clauses =
            [QueryClause.Projection, QueryClause.Where, QueryClause.OrderBy, QueryClause.SliceProjection];
        foreach (QueryClause clause in clauses)
        {
            string query = DicBuilder.TryGetValue(clause, out StringBuilder? built) switch
            {
                true => built.ToString(),
                false => string.Empty
            };

            if (string.IsNullOrEmpty(query) && clause == QueryClause.Projection)
            {
                const string sqlSelectClause = " SELECT {0} FROM {1}s ";
                string[] propsName = Properties.Select(p => p.Name).ToArray();
                string columns = string.Join(", ", propsName);
                string table = Entity.Name;
                StringBuilder sqlBuilder = new();
                sqlBuilder.AppendFormat(sqlSelectClause, columns, table);
                query = sqlBuilder.ToString();
            }

            Builder.Append(query);
        }
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

        DicBuilder.AddOrUpdate(QueryClause.Where, new StringBuilder($" WHERE {query}"),
            (_, built) => built.Append(query));
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

        DicBuilder.AddOrUpdate(QueryClause.Where, new StringBuilder($" WHERE {query}"),
            (_, built) => built.Append(query));
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

        DicBuilder.AddOrUpdate(QueryClause.Where, new StringBuilder($" WHERE {query}"),
            (_, built) => built.Append(query));
    }

    public void Visit(ICombinedFilterDefinition filters)
    {
        (LogicalOperator logicalOperator, IQuerying[] filterDefinitions) =
            filters.GroupingFilterDefinition;

        Join(QueryClause.Where, QueryBuildHelper.LogicalOperators[logicalOperator], filterDefinitions);
    }

    public void Visit(ISortDefinition filterDefinition)
    {
        (SortDirection sortDirection, string fieldName) =
            filterDefinition.OrderParameter;

        Guard.Against.Null(fieldName);

        string query = string.Join(' ',
            [fieldName, QueryBuildHelper.OrderByOperators[sortDirection]]);

        DicBuilder.AddOrUpdate(QueryClause.OrderBy, new StringBuilder($" ORDER BY {query}"),
            (_, built) => built.Append(query));
    }

    public void Visit(ICombinedSortDefinition sorts)
    {
        Join(QueryClause.OrderBy, ", ", sorts.Sorts);
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
                Columns.AddRange(Properties.Select(p => $"[{p.Name}]").ToArray());
            }

            Columns.Remove(field.FieldName);
        }
    }

    public void Visit(ISliceProjectionDefinition sliceProjectionDefinition)
    {
        StringBuilder builder = new();
        if (sliceProjectionDefinition.Limit > 0)
        {
            builder.Append($" LIMIT {sliceProjectionDefinition.Limit} ");
        }

        if (sliceProjectionDefinition.Skip > 0)
        {
            builder.Append($" OFFSET {sliceProjectionDefinition.Skip} ");
        }

        DicBuilder.AddOrUpdate(QueryClause.SliceProjection,
            builder,
            (_, _) => builder);
    }

    public void Visit(ICombinedProjectionDefinition combinedProjectionDefinition)
    {
        Join(QueryClause.Projection, string.Empty, combinedProjectionDefinition.Projections);
        const string sqlSelectClause = " {0} FROM {1}s ";
        string columns = string.Join(", ", Columns);
        string table = Entity.Name;
        StringBuilder builder = new();
        builder.AppendFormat(sqlSelectClause, columns, table);
        string query = builder.ToString();
        DicBuilder.AddOrUpdate(QueryClause.Projection, new StringBuilder($" SELECT {query} "),
            (_, built) => built.Insert(0, " SELECT ").Append(query));
    }
}