namespace KISS.FluentSqlBuilder.Core;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
/// <typeparam name="TReturn">
///     The type of objects to return, representing the query result rows.
///     This type must match the structure of the query results.
/// </typeparam>
public sealed partial record QueryOperator<TReturn>
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

    private Type? InEntityType { get; set; }

    private static Type OutEntityType { get; } = typeof(TReturn);

    private ParameterExpression CurrentEntityExVariable { get; set; } = Expression.Variable(OutEntityType, "CurrentEntityExVariable");

    private static Type OutEntitiesType { get; } = typeof(List<TReturn>);

    private ParameterExpression OutEntitiesExVariable { get; set; } = Expression.Variable(OutEntitiesType, "OutEntitiesExVariable");

    private ParameterExpression? OutDictEntityTypeExVariable { get; set; }

    private static Type OutDictKeyType { get; } = typeof(object);

    private ParameterExpression? OutDictKeyExVariable { get; set; }

    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    private Type? OutDictEntityType { get; set; }

    private BinaryExpression InitializeOutputVariable => Expression.Assign(OutEntitiesExVariable, Expression.New(OutEntitiesType));

    private ParameterExpression CurrentEntryExParameter => Composite.CurrentEntryExParameter;

    private static Type InEntryIterType { get; } = typeof(IEnumerator<IDictionary<string, object>>);

    private static ParameterExpression InEntryIterExVariable { get; } = Expression.Variable(InEntryIterType, "InEntryIterExVariable");

    private static MethodCallExpression MoveNextOnInEntryEnumerator { get; } = Expression.Call(InEntryIterExVariable, IterMoveNextMethod);

    private static MethodCallExpression DisposeInEntryEnumerator { get; } = Expression.Call(InEntryIterExVariable, DisposeMethod);

    private static LabelTarget BreakLabel { get; } = Expression.Label();

    private static GotoExpression ExitsLoop { get; } = Expression.Break(BreakLabel);

    private BinaryExpression AssignCurrentInputRowFromInputEnumerator
        => Expression.Assign(
            CurrentEntryExParameter,
            Expression.Property(InEntryIterExVariable, "Current"));

    /// <summary>
    ///     Gets the dictionary that maps grouping key names to their types.
    ///     This collection is used to maintain type information for
    ///     grouping operations in the query.
    /// </summary>
    public Dictionary<string, Type> GroupingKeys
        => Composite.GroupingKeys;

    /// <summary>
    ///     Gets the dictionary that maps aggregation key names to their types.
    ///     This collection is used to maintain type information for
    ///     aggregation operations in the query.
    /// </summary>
    public Dictionary<string, Type> AggregationKeys
        => Composite.AggregationKeys;

    private BinaryExpression SetupInputDataEnumerator
        => Expression.Assign(
                InEntryIterExVariable,
                Expression.Call(Expression.Constant(InputData), GetEnumeratorForIEnumDictStrObj));
}
