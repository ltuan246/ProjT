namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TIn">
///     The type representing the database record set (input entity).
///     Used as the source type for query construction and mapping.
/// </typeparam>
/// <typeparam name="TOut">
///     The combined type to return as the result of the query.
///     Used for type-safe mapping of query results.
/// </typeparam>
public sealed partial record CompositeQuery<TIn, TOut> : IComposite
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CompositeQuery{TIn, TOut}" /> class.
    ///     Sets up the core expression variables for output entity and collection processing.
    /// </summary>
    public CompositeQuery()
    {
        OutEntitiesExVariable = Expression.Variable(OutEntitiesType, "OutEntitiesExVariable");
        CurrentEntityExVariable = Expression.Variable(OutEntityType, "CurrentEntityExVariable");
        IndexerExVariable = Expression.Variable(OutEntityType, "IndexerExVariable");
    }

    /// <inheritdoc />
    public Type InEntityType { get; } = typeof(TIn);

    /// <inheritdoc />
    public Type OutEntityType { get; } = typeof(TOut);

    /// <inheritdoc />
    public Type OutEntitiesType { get; } = typeof(List<TOut>);
}
