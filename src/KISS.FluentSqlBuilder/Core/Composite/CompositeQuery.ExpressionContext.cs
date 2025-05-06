namespace KISS.FluentSqlBuilder.Core.Composite;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
public sealed partial class CompositeQuery
{
    /// <summary>
    ///     A function that define how to process each row.
    /// </summary>
    public Func<(ParameterExpression IterRowParameter, ParameterExpression CurrentEntityVariable), Expression>
        IterRowProcessor
    { get; set; } = default!;

    /// <summary>
    ///     A function that define how to process each row.
    /// </summary>
    public List<Func<(ParameterExpression IterRowParameter, IndexExpression Indexer), Expression>>
        JoinRowProcessors
    { get; } = [];
}
