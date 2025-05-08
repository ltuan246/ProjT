namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
public sealed partial record CompositeQuery
{
    /// <summary>
    /// CurrentEntryExParameter.
    /// </summary>
    public ParameterExpression CurrentEntryExParameter { get; } =
        Expression.Parameter(TypeUtils.DapperRowType, "CurrentEntryExParameter");

    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    public ParameterExpression? OutDictEntityTypeExVariable { get; set; }

    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    public ParameterExpression OutDictKeyExVariable { get; } = Expression.Variable(TypeUtils.ObjType, "OutDictKeyExVariable");

    /// <summary>
    ///     A function that define how to process each row.
    /// </summary>
    public List<Expression> JoinRowProcessors { get; } = [];
}
