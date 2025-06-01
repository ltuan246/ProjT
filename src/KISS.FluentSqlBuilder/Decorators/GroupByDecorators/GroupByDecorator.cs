namespace KISS.FluentSqlBuilder.Decorators.GroupByDecorators;

/// <summary>
///     Implements a decorator for SQL GROUP BY operations, enabling grouping and aggregation
///     logic in dynamically constructed queries. Handles the creation of expression variables,
///     type mappings, and SQL generation for grouped results.
/// </summary>
public sealed partial record GroupByDecorator : QueryDecorator
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GroupByDecorator" /> class,
    ///     configuring types and expression variables for grouping and aggregation.
    /// </summary>
    /// <param name="inner">The inner composite query component to decorate.</param>
    public GroupByDecorator(IComposite inner)
        : base(inner)
    {
        OutDictEntitiesType = TypeUtils.DictionaryType.MakeGenericType([OuterKeyType, Inner.OutEntitiesType]);
        InnerDictObjEntityType = TypeUtils.DictionaryType.MakeGenericType([TypeUtils.ObjType, Inner.OutEntityType]);

        Type[] outerTypeArguments = [OuterKeyType, InnerDictObjEntityType];

        OuterDictTupleEntityCollectionType = TypeUtils.DictionaryType.MakeGenericType(outerTypeArguments);
        OuterIterType = TypeUtils.DictionaryEnumeratorType.MakeGenericType(outerTypeArguments);
        OuterEntryType = TypeUtils.KeyValuePairType.MakeGenericType(outerTypeArguments);

        // Expression variables for output and iteration in grouping logic.
        OutputVariable = Expression.Variable(OutDictEntitiesType, "OutputVariable");
        OuterDictObjEntityVariable =
            Expression.Variable(OuterDictTupleEntityCollectionType, "OuterDictTupleEntityCollectionType");
        OuterKeyVariable = Expression.Variable(OuterKeyType, "OuterKeyVariable");
        InnerKeyVariable = Expression.Variable(InnerKeyType, "InnerKeyVariable");
        OuterDictIterVariable = Expression.Variable(OuterIterType, "OuterDictIterVariable");
        OuterDictEntryParameter = Expression.Parameter(OuterEntryType, "OuterDictEntryParameter");
    }

    /// <summary>
    ///     Gets the type representing the dictionary of grouped output entities.
    /// </summary>
    public Type OutDictEntitiesType { get; init; }

    /// <summary>
    ///     Gets the type representing the outer dictionary for tuple entity collections.
    /// </summary>
    public Type OuterDictTupleEntityCollectionType { get; init; }

    /// <summary>
    ///     Gets the type used for iterating over the outer dictionary.
    /// </summary>
    public Type OuterIterType { get; init; }

    /// <summary>
    ///     Gets the type representing an entry in the outer dictionary.
    /// </summary>
    public Type OuterEntryType { get; init; }

    /// <summary>
    ///     Gets the type used as the key for the outer dictionary (typically a tuple).
    /// </summary>
    public Type OuterKeyType { get; } = TypeUtils.TupleType;

    /// <summary>
    ///     Gets the type representing the inner dictionary of grouped entities.
    /// </summary>
    public Type InnerDictObjEntityType { get; init; }

    /// <summary>
    ///     Gets the type used as the key for the inner dictionary (typically an object).
    /// </summary>
    public Type InnerKeyType { get; } = TypeUtils.ObjType;

    /// <summary>
    ///     Gets the dictionary mapping grouping key names to their types.
    ///     Used to define the grouping columns and their types in the query.
    /// </summary>
    public Dictionary<string, Type> GroupingKeys { get; } = [];

    /// <summary>
    ///     Gets the dictionary mapping aggregation key names to their types.
    ///     Used to define the aggregation columns and their types in the query.
    /// </summary>
    public Dictionary<string, Type> AggregationKeys { get; } = [];

    /// <inheritdoc />
    public override string Sql
    {
        get
        {
            // Build the SQL query using CTE and GROUP BY logic.
            Append("WITH CommonTableExpression AS (");
            AppendLine(Inner.Sql);
            AppendLine(")");

            new EnumeratorProcessor<string>(SqlStatements[SqlStatement.GroupBy])
                .AccessFirst(_ =>
                {
                    StringBuilder outerSelectBuilder = new(),
                        innerSelectBuilder = new(),
                        groupByFilteringBuilder = new(),
                        onClauseBuilder = new();

                    // Build SELECT clause for aggregation keys.
                    new EnumeratorProcessor<KeyValuePair<string, Type>>(AggregationKeys)
                        .AccessFirst(kv =>
                        {
                            outerSelectBuilder.Append($"GP.{kv.Key}");
                        })
                        .AccessRemaining(kv =>
                        {
                            outerSelectBuilder.AppendLine($", GP.{kv.Key}");
                        })
                        .AccessLast(() => outerSelectBuilder.Append(','))
                        .Execute();

                    // Build SELECT, GROUP BY, and ON clauses for grouping keys.
                    new EnumeratorProcessor<KeyValuePair<string, Type>>(GroupingKeys)
                        .AccessFirst(kv =>
                        {
                            outerSelectBuilder.Append($"GP.{kv.Key}");
                            innerSelectBuilder.Append($"{kv.Key}");
                            groupByFilteringBuilder.Append($"GROUP BY {kv.Key}");
                            onClauseBuilder.Append($"CTE.{kv.Key} = GP.{kv.Key}");
                        })
                        .AccessRemaining(kv =>
                        {
                            outerSelectBuilder.AppendLine($", GP.{kv.Key}");
                            innerSelectBuilder.AppendLine($", {kv.Key}");
                            groupByFilteringBuilder.AppendLine($", {kv.Key}");
                            onClauseBuilder.AppendLine(
                                $"AND CTE.{kv.Key} = GP.{kv.Key}");
                        })
                        .AccessLast(() => outerSelectBuilder.Append(','))
                        .Execute();

                    outerSelectBuilder.Append("CTE.*");

                    // Build SELECT clause for aggregate functions.
                    new EnumeratorProcessor<string>(SqlStatements[SqlStatement.SelectAggregate])
                        .AccessFirst(fs =>
                        {
                            if (innerSelectBuilder.Length > 0)
                            {
                                innerSelectBuilder.Append(',');
                            }

                            innerSelectBuilder.Append($" {fs}");
                        })
                        .AccessRemaining(fs =>
                        {
                            innerSelectBuilder.AppendLine($", {fs}");
                        })
                        .Execute();

                    // Build HAVING clause for group filtering.
                    new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Having])
                        .AccessFirst(fs =>
                        {
                            groupByFilteringBuilder.Append($" HAVING {fs}");
                        })
                        .AccessRemaining(fs =>
                        {
                            groupByFilteringBuilder.AppendLine($"AND {fs}");
                        })
                        .Execute();

                    // Compose the final SQL query for grouped results.
                    Append($@"
                        SELECT
                            {outerSelectBuilder}
                        FROM CommonTableExpression CTE
                        JOIN (
                            SELECT
                                {innerSelectBuilder}
                            FROM CommonTableExpression
                                {groupByFilteringBuilder}
                        ) GP
                        ON {onClauseBuilder};
                    ");

                    AppendLine();
                })
                .Execute();

            return SqlBuilder.ToString();
        }
    }
}
