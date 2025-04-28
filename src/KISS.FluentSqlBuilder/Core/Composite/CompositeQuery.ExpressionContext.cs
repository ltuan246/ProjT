namespace KISS.FluentSqlBuilder.Core.Composite;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
public sealed partial class CompositeQuery
{
    private static MethodInfo IterMoveNextMethod { get; } = typeof(IEnumerator).GetMethod("MoveNext", Type.EmptyTypes)!;

    private static MethodInfo DisposeMethod { get; } = typeof(IDisposable).GetMethod("Dispose", Type.EmptyTypes)!;

    /// <summary>
    ///     Gets the MethodInfo for the GetEnumerator method of <see cref="IEnumerable{IDictionary}" />.
    ///     This is used to create an enumerator for iterating over collections of dictionaries in expression trees.
    /// </summary>
    /// <remarks>
    ///     Cached as a static property to avoid repeated reflection calls,
    ///     improving performance in expression tree construction.
    ///     The specific type
    ///     <see cref="IEnumerable{IDictionary}" /> ensures compatibility with dictionary-based data rows.
    /// </remarks>
    private static MethodInfo GetEnumeratorForIEnumDictStrObj { get; } =
        typeof(IEnumerable<IDictionary<string, object>>).GetMethod("GetEnumerator")!;

    private static Type InEntryType { get; } = typeof(IDictionary<string, object>);

    private static ParameterExpression CurrentEntryExParameter { get; } = Expression.Parameter(InEntryType, "currentInputRowParameter");

    private static Type InEntryIterType { get; } = typeof(IEnumerator<IDictionary<string, object>>);

    private static ParameterExpression InEntryIterExVariable { get; } = Expression.Variable(InEntryIterType, "inputDictIterVariable");

    private static MethodCallExpression MoveNextOnInEntryEnumerator { get; } = Expression.Call(InEntryIterExVariable, IterMoveNextMethod);

    private static MethodCallExpression DisposeInEntryEnumerator { get; } = Expression.Call(InEntryIterExVariable, DisposeMethod);

    private static LabelTarget BreakLabel { get; } = Expression.Label();

    private static GotoExpression ExitsLoop { get; } = Expression.Break(BreakLabel);

    private static BinaryExpression AssignCurrentInputRowFromInputEnumerator { get; } = Expression.Assign(
        CurrentEntryExParameter,
        Expression.Property(InEntryIterExVariable, "Current"));

    private BlockExpression ProcessRowsIfExist { get; set; } = Expression.Block();

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

    private TryExpression WhileBlock()
        => Expression.TryFinally(
                Expression.Loop(
                    Expression.IfThenElse(
                        MoveNextOnInEntryEnumerator, // If MoveNext returns true (more rows),
                        ProcessRowsIfExist,
                        ExitsLoop), // Otherwise, break out of the loop
                    BreakLabel),
                DisposeInEntryEnumerator);
}
