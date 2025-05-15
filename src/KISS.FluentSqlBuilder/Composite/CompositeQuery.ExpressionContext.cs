namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TIn">The type representing the database record set.</typeparam>
/// <typeparam name="TOut">The combined type to return.</typeparam>
public sealed partial record CompositeQuery<TIn, TOut>
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

    /// <inheritdoc />
    public BlockExpression Block { get; } = Expression.Block();
}
