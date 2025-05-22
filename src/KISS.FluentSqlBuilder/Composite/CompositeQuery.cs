namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TIn">The type representing the database record set.</typeparam>
/// <typeparam name="TOut">The combined type to return.</typeparam>
public sealed partial record CompositeQuery<TIn, TOut> : IComposite
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CompositeQuery{TIn, TOut}"/> class.
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