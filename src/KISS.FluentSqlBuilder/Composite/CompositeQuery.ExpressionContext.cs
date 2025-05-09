namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
/// <typeparam name="TIn">The type representing the database record set.</typeparam>
/// <typeparam name="TOut">The combined type to return.</typeparam>
public sealed partial record CompositeQuery<TIn, TOut>
{
    /// <inheritdoc />
    public Type InEntityType { get; } = typeof(TIn);

    /// <inheritdoc />
    public ParameterExpression CurrentEntityExVariable { get; } =
        Expression.Variable(typeof(TOut), "CurrentEntityExVariable");

    /// <inheritdoc />
    public ParameterExpression OutEntitiesExVariable { get; } =
        Expression.Variable(typeof(List<TOut>), "OutEntitiesExVariable");

    /// <inheritdoc />
    public ParameterExpression InEntryIterExVariable { get; } =
        Expression.Variable(TypeUtils.DapperRowIteratorType, "InEntryIterExVariable");

    /// <inheritdoc />
    public ParameterExpression CurrentEntryExParameter { get; } =
        Expression.Parameter(TypeUtils.DapperRowType, "CurrentEntryExParameter");

    /// <inheritdoc />
    public ParameterExpression OutDictEntityTypeExVariable { get; } =
        Expression.Variable(typeof(Dictionary<object, TOut>), "OutDictEntityTypeExVariable");

    /// <inheritdoc />
    public ParameterExpression OutDictKeyExVariable { get; } =
        Expression.Variable(TypeUtils.ObjType, "OutDictKeyExVariable");

    /// <inheritdoc />
    public List<Expression> JoinRowProcessors { get; } = [];
}
