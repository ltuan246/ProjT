namespace KISS.FluentSqlBuilder.Decorators.JoinDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TIn">The type representing the database record set.</typeparam>
/// <typeparam name="TOut">The combined type to return.</typeparam>
public sealed partial record JoinDecorator<TIn, TOut>
{
    /// <inheritdoc />
    public ParameterExpression InEntriesExParameter { get; init; }

    /// <inheritdoc />
    public ParameterExpression InEntriesExVariable { get; init; }

    /// <inheritdoc />
    public ParameterExpression OutEntitiesExVariable { get; init; }

    /// <inheritdoc />
    public ParameterExpression CurrentEntryExVariable { get; init; }

    /// <inheritdoc />
    public ParameterExpression CurrentEntityExVariable { get; init; }

    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    private ParameterExpression OutDictEntityTypeExVariable { get; init; }

    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    private ParameterExpression OutDictKeyExVariable { get; init; }

    /// <summary>
    /// OutDictKeyAccessorExVariable.
    /// </summary>
    private ParameterExpression OutDictKeyAccessorExVariable { get; init; }

    /// <summary>
    ///     A function that define how to process each row.
    /// </summary>
    private List<Expression> JoinRowProcessors { get; } = [];
}