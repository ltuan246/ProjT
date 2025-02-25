namespace KISS.FluentSqlBuilder.QueryHandlerChain;

/// <summary>
///     CompositeQuery.
/// </summary>
public sealed partial class CompositeQuery
{
    /// <summary>
    ///     Used to represent an instance of the source entity in the expression tree.
    /// </summary>
    public ParameterExpression SourceParameter { get; set; } = default!;

    /// <summary>
    ///      Used to store the constructed target entity during the execution of the expression tree.
    /// </summary>
    public ParameterExpression RetrieveVariable { get; set; } = default!;

    /// <summary>
    /// Represents a row in an iteration loop.
    /// </summary>
    public ParameterExpression IterationRowVariable { get; } = Expression.Parameter(typeof(IDictionary<string, object>), "iterator");
}