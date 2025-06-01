namespace KISS.FluentSqlBuilder.Decorators.GroupByDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public sealed partial record GroupByDecorator : QueryDecorator
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GroupByDecorator"/> class.
    /// </summary>
    /// <param name="inner">inner.</param>
    public GroupByDecorator(IComposite inner)
        : base(inner)
    {
        OutDictEntitiesType = TypeUtils.DictionaryType.MakeGenericType([OuterKeyType, Inner.OutEntitiesType]);
        InnerDictObjEntityType = TypeUtils.DictionaryType.MakeGenericType([TypeUtils.ObjType, Inner.OutEntityType]);

        Type[] outerTypeArguments = [OuterKeyType, InnerDictObjEntityType];

        OuterDictTupleEntityCollectionType = TypeUtils.DictionaryType.MakeGenericType(outerTypeArguments);
        OuterIterType = TypeUtils.DictionaryEnumeratorType.MakeGenericType(outerTypeArguments);
        OuterEntryType = TypeUtils.KeyValuePairType.MakeGenericType(outerTypeArguments);

        OutputVariable = Expression.Variable(OutDictEntitiesType, "OutputVariable");
        OuterDictObjEntityVariable = Expression.Variable(OuterDictTupleEntityCollectionType, "OuterDictTupleEntityCollectionType");
        OuterKeyVariable = Expression.Variable(OuterKeyType, "OuterKeyVariable");
        InnerKeyVariable = Expression.Variable(InnerKeyType, "InnerKeyVariable");
        OuterDictIterVariable = Expression.Variable(OuterIterType, "OuterDictIterVariable");
        OuterDictEntryParameter = Expression.Parameter(OuterEntryType, "OuterDictEntryParameter");
    }

    /// <summary>
    /// OutEntitiesType.
    /// </summary>
    public Type OutDictEntitiesType { get; init; }

    /// <summary>
    /// OutEntitiesType.
    /// </summary>
    public Type OuterDictTupleEntityCollectionType { get; init; }

    /// <summary>
    /// OutEntitiesType.
    /// </summary>
    public Type OuterIterType { get; init; }

    /// <summary>
    /// OutEntitiesType.
    /// </summary>
    public Type OuterEntryType { get; init; }

    /// <summary>
    /// OutEntitiesType.
    /// </summary>
    public Type OuterKeyType { get; } = TypeUtils.TupleType;

    /// <summary>
    /// OutEntitiesType.
    /// </summary>
    public Type InnerDictObjEntityType { get; init; }

    /// <summary>
    /// OutEntitiesType.
    /// </summary>
    public Type InnerKeyType { get; } = TypeUtils.ObjType;

    /// <summary>
    ///     Gets the dictionary that maps grouping key names to their types.
    ///     This collection is used to maintain type information for
    ///     grouping operations in the query.
    /// </summary>
    public Dictionary<string, Type> GroupingKeys { get; } = [];

    /// <summary>
    ///     Gets the dictionary that maps aggregation key names to their types.
    ///     This collection is used to maintain type information for
    ///     aggregation operations in the query.
    /// </summary>
    public Dictionary<string, Type> AggregationKeys { get; } = [];

    /// <inheritdoc />
    public override string Sql
    {
        get
        {
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