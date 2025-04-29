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

    private static Type OutEntityType { get; } = typeof(TReturn);

    private static ParameterExpression CurrentEntityExVariable { get; } = Expression.Variable(OutEntityType, "CurrentEntityExVariable");

    private static Type OutEntitiesType { get; } = typeof(List<TReturn>);

    private static ParameterExpression OutEntitiesExVariable { get; } = Expression.Variable(OutEntitiesType, "OutEntitiesExVariable");

    private static Type OutDictEntityType { get; } = typeof(Dictionary<object, TReturn>);

    private static ParameterExpression OutDictEntityTypeExVariable { get; } = Expression.Variable(OutDictEntityType, "OutDictEntityTypeExVariable");

    private static Type OutDictKeyType { get; } = typeof(object);

    private static ParameterExpression OutDictKeyExVariable { get; } = Expression.Variable(OutDictKeyType, "OutDictKeyExVariable");

    private static IndexExpression OutKeyAccessor { get; } = Expression.MakeIndex(
        OutDictEntityTypeExVariable,
        OutDictEntityType.GetProperty("Item")!,
        [OutDictKeyExVariable]);

    private static ConstantExpression PrimaryKey { get; } = Expression.Constant("Extend0_Id");

    private static BinaryExpression InitializeDictVariable { get; } = Expression.Assign(
        OutDictEntityTypeExVariable,
        Expression.New(OutDictEntityType));

    private static BinaryExpression InitializeOutputVariable { get; } = Expression.Assign(OutEntitiesExVariable, Expression.New(OutEntitiesType));

    private static Type InEntryType { get; } = typeof(IDictionary<string, object>);

    private static ParameterExpression CurrentEntryExParameter { get; } = Expression.Parameter(InEntryType, "CurrentEntryExParameter");

    private static Type InEntryIterType { get; } = typeof(IEnumerator<IDictionary<string, object>>);

    private static ParameterExpression InEntryIterExVariable { get; } = Expression.Variable(InEntryIterType, "InEntryIterExVariable");

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
        => Composite.IterRowProcessor;

    /// <summary>
    ///     A function that define how to process each row.
    /// </summary>
    public List<Expression> ApplyJoinProcessorsToInnerKeyAccessor
        => [.. Composite.JoinRowProcessors.Select(processor => processor((CurrentEntryExParameter, OutKeyAccessor)))];

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

    private TryExpression WhileBlock
        => Expression.TryFinally(
                Expression.Loop(
                    Expression.IfThenElse(
                        MoveNextOnInEntryEnumerator, // If MoveNext returns true (more rows),
                        ProcessRowsIfExist,
                        ExitsLoop), // Otherwise, break out of the loop
                    BreakLabel),
                DisposeInEntryEnumerator);

    private BinaryExpression AssignKeyVariableFromPrimaryKey
        => Expression.Assign(
            OutDictKeyExVariable,
            ChangeType(
                Expression.Property(CurrentEntryExParameter, "Item", PrimaryKey),
                OutDictKeyType));

    // Processes and adds the entity to the dictionary if the key is new.
    private ConditionalExpression InitializeEntityIfKeyMissing
        => Expression.IfThen(
            Expression.Not(Expression.Call(
                OutDictEntityTypeExVariable,
                OutDictEntityType.GetMethod("ContainsKey")!,
                OutDictKeyExVariable)),
            Expression.Block(
                [], // Ensures variables is scoped for this operation
                    // Applies the row processor to construct or modify the entity.
                IterRowProcessor((CurrentEntryExParameter, CurrentEntityExVariable)),
                // Adds the processed entity to the dictionary with its key.
                Expression.Call(
                    OutDictEntityTypeExVariable,
                    OutDictEntityType.GetMethod("Add")!,
                    OutDictKeyExVariable,
                    CurrentEntityExVariable)));

    private MethodCallExpression ProcessAddValuesToOutput
        => Expression.Call(
            OutEntitiesExVariable,
            OutEntitiesType.GetMethod("AddRange")!,
            Expression.Property(OutDictEntityTypeExVariable, "Values"));

    private BlockExpression ProcessCurrentInputRowToOutputEntity
        => Expression.Block(
            [], // Defines a block with the current entity variable.
            IterRowProcessor((CurrentEntryExParameter, CurrentEntityExVariable)),
            Expression.Call(
                OutEntitiesExVariable, // Calls the Add method on the output list.
                OutEntitiesType.GetMethod("Add")!, // Retrieves the Add method via reflection.
                CurrentEntityExVariable));
}
