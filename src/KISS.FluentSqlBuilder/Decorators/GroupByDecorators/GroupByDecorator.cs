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
        var outDictEntitiesType = TypeUtils.DictionaryType.MakeGenericType([TypeUtils.TupleType, Inner.OutEntitiesType]);
        InnerDictObjEntityType = TypeUtils.DictionaryType.MakeGenericType([TypeUtils.ObjType, Inner.OutEntityType]);

        Type[] outerTypeArguments = [TypeUtils.TupleType, InnerDictObjEntityType];

        var outerDictTupleEntityCollectionType = TypeUtils.DictionaryType.MakeGenericType(outerTypeArguments);
        var outerIterType = TypeUtils.DictionaryEnumeratorType.MakeGenericType(outerTypeArguments);
        var outerEntryType = TypeUtils.KeyValuePairType.MakeGenericType(outerTypeArguments);

        // Expression variables for output and iteration in grouping logic.
        OutputVariable = Expression.Variable(outDictEntitiesType, "OutputVariable");
        OuterDictObjEntityVariable =
            Expression.Variable(outerDictTupleEntityCollectionType, "OuterDictTupleEntityCollectionType");
        OuterKeyVariable = Expression.Variable(TypeUtils.TupleType, "OuterKeyVariable");
        InnerKeyVariable = Expression.Variable(TypeUtils.ObjType, "InnerKeyVariable");
        OuterDictIterVariable = Expression.Variable(outerIterType, "OuterDictIterVariable");
        OuterDictEntryParameter = Expression.Parameter(outerEntryType, "OuterDictEntryParameter");
    }

    /// <summary>
    ///     Gets the type representing the inner dictionary of grouped entities.
    /// </summary>
    public Type InnerDictObjEntityType { get; init; }

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
}
