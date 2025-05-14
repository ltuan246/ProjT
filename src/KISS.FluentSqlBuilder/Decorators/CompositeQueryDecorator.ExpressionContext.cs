namespace KISS.FluentSqlBuilder.Decorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TIn">The type representing the database record set.</typeparam>
/// <typeparam name="TOut">The combined type to return.</typeparam>
public sealed partial record CompositeQueryDecorator
{
    /// <inheritdoc />
    public Type InEntityType { get; } = InnerComposite.InEntityType;

    /// <inheritdoc />
    public ParameterExpression CurrentEntityExVariable { get; } = InnerComposite.CurrentEntityExVariable;

    /// <inheritdoc />
    public ParameterExpression OutEntitiesExVariable { get; } = InnerComposite.OutEntitiesExVariable;

    /// <inheritdoc />
    public ParameterExpression InEntriesExVariable { get; } = InnerComposite.InEntriesExVariable;

    /// <inheritdoc />
    public ParameterExpression CurrentEntryExVariable { get; } = InnerComposite.CurrentEntryExVariable;

    /// <inheritdoc />
    public ParameterExpression OutDictEntityTypeExVariable { get; } = InnerComposite.OutDictEntityTypeExVariable;

    /// <inheritdoc />
    public ParameterExpression OutDictKeyAccessorExVariable { get; } = InnerComposite.OutDictKeyAccessorExVariable;

    /// <inheritdoc />
    public ParameterExpression OutDictKeyExVariable { get; } = InnerComposite.OutDictKeyExVariable;

    /// <inheritdoc />
    public List<Expression> JoinRowProcessors { get; } = InnerComposite.JoinRowProcessors;
}
