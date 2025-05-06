namespace KISS.FluentSqlBuilder.Core.Composite;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
public sealed partial class CompositeQuery
{
    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    public ParameterExpression? OutDictEntityTypeExVariable { get; set; }

    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    public ParameterExpression? OutDictKeyExVariable { get; set; }

    /// <summary>
    ///     A function that define how to process each row.
    /// </summary>
    public List<Expression> JoinRowProcessors { get; } = [];
}
