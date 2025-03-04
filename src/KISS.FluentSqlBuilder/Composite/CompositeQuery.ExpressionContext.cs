namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
public sealed partial class CompositeQuery
{
    private static MethodInfo ItorMoveNextMethod { get; } = typeof(IEnumerator).GetMethod("MoveNext")!;

    private static MethodInfo DisposeMethod { get; } = typeof(IDisposable).GetMethod("Dispose")!;

    /// <summary>
    ///     Gets the MethodInfo for the GetEnumerator method of <see cref="IEnumerable{IDictionary{string, object}}" />.
    ///     This is used to create an enumerator for iterating over collections of dictionaries in expression trees.
    /// </summary>
    /// <remarks>
    ///     Cached as a static property to avoid repeated reflection calls,
    ///     improving performance in expression tree construction.
    ///     The specific type
    ///     <see cref="IEnumerable{IDictionary{string, object}}" /> ensures compatibility with dictionary-based data rows.
    /// </remarks>
    private static MethodInfo GetEnumeratorForIEnumDictStrObj { get; } =
        typeof(IEnumerable<IDictionary<string, object>>).GetMethod("GetEnumerator")!;

    /// <summary>
    ///     Gets the MethodInfo for the indexer (get_Item) of IDictionary{string, object}.
    ///     This is used to access values by key in dictionary rows within expression trees.
    /// </summary>
    /// <remarks>
    ///     Cached statically for efficiency, targeting IDictionary{string, object} to match the expected row type.
    ///     The indexer returns an object, requiring type conversion in downstream processing.
    /// </remarks>
    private static MethodInfo GetIndexerForDictStrObj { get; } =
        typeof(IDictionary<string, object>).GetMethod("get_Item")!;

    /// <summary>
    ///     A function that define how to process each row.
    /// </summary>
    public Func<(ParameterExpression IterRowParameter, ParameterExpression CurrentEntityVariable), Expression> IterRowProcessor { get; set; } = default!;

    /// <summary>
    ///     A function that define how to process each row.
    /// </summary>
    public List<Func<(ParameterExpression IterRowParameter, IndexExpression Indexer), Expression>> JoinRowProcessors { get; } = [];
}
