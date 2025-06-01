namespace KISS.FluentSqlBuilder.Decorators.GroupByDecorators;

/// <summary>
///     Contains expression variables used for constructing and executing grouping logic
///     in the GroupByDecorator. These variables represent intermediate states and iterators
///     required for building LINQ expression trees for SQL GROUP BY operations.
/// </summary>
public sealed partial record GroupByDecorator
{
    /// <summary>
    ///     Expression variable representing the output dictionary for grouped entities.
    /// </summary>
    public ParameterExpression OutputVariable { get; init; }

    /// <summary>
    ///     Expression variable representing the outer dictionary object entity in grouping logic.
    /// </summary>
    public ParameterExpression OuterDictObjEntityVariable { get; init; }

    /// <summary>
    ///     Expression variable representing the key for the outer dictionary (group key).
    /// </summary>
    public ParameterExpression OuterKeyVariable { get; init; }

    /// <summary>
    ///     Expression variable representing the key for the inner dictionary (aggregation key).
    /// </summary>
    public ParameterExpression InnerKeyVariable { get; init; }

    /// <summary>
    ///     Expression variable used as an iterator for the outer dictionary during grouping.
    /// </summary>
    public ParameterExpression OuterDictIterVariable { get; init; }

    /// <summary>
    ///     Expression parameter representing an entry in the outer dictionary during iteration.
    /// </summary>
    public ParameterExpression OuterDictEntryParameter { get; init; }
}
