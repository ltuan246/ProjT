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
    private Type? InEntityType { get; set; }

    private static Type OutEntityType { get; } = typeof(TReturn);

    private ParameterExpression CurrentEntityExVariable { get; set; } =
        Expression.Variable(OutEntityType, "CurrentEntityExVariable");

    private static Type OutEntitiesType { get; } = typeof(List<TReturn>);

    private ParameterExpression OutEntitiesExVariable { get; set; } =
        Expression.Variable(OutEntitiesType, "OutEntitiesExVariable");

    private ParameterExpression OutDictEntityTypeExVariable => Composite.OutDictEntityTypeExVariable!;

    private ParameterExpression OutDictKeyExVariable => Composite.OutDictKeyExVariable;

    private BinaryExpression InitializeOutputVariable =>
        Expression.Assign(OutEntitiesExVariable, Expression.New(OutEntitiesType));

    private ParameterExpression CurrentEntryExParameter => Composite.CurrentEntryExParameter;

    private ParameterExpression InEntryIterExVariable { get; } =
        Expression.Variable(TypeUtils.DapperRowIteratorType, "InEntryIterExVariable");

    private MethodCallExpression MoveNextOnInEntryEnumerator
        => Expression.Call(InEntryIterExVariable, TypeUtils.IterMoveNextMethod);

    private MethodCallExpression DisposeInEntryEnumerator
        => Expression.Call(InEntryIterExVariable, TypeUtils.DisposeMethod);

    private static LabelTarget BreakLabel { get; } = Expression.Label();

    private static GotoExpression ExitsLoop { get; } = Expression.Break(BreakLabel);

    private BinaryExpression AssignCurrentInputRowFromInputEnumerator
        => Expression.Assign(
            CurrentEntryExParameter,
            Expression.Property(InEntryIterExVariable, "Current"));

    private BinaryExpression SetupInputDataEnumerator
        => Expression.Assign(
            InEntryIterExVariable,
            Expression.Call(Expression.Constant(InputData), TypeUtils.GetEnumeratorForIEnumDictStrObj));
}
