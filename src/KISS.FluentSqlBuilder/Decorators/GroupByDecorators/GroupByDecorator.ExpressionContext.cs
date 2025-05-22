namespace KISS.FluentSqlBuilder.Decorators.GroupByDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public sealed partial record GroupByDecorator
{
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
    public Type OuterKeyType { get; } = typeof(ITuple);

    /// <summary>
    /// OutEntitiesType.
    /// </summary>
    public Type InnerDictObjEntityType { get; init; }

    /// <summary>
    /// OutEntitiesType.
    /// </summary>
    public Type InnerKeyType { get; } = typeof(object);

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
}
