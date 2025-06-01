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
    /// <summary>
    ///     Gets the parameter expression representing the input collection of Dapper rows.
    ///     Used as the entry point for iterating over the input data set.
    /// </summary>
    public ParameterExpression InEntriesExParameter { get; } =
        Expression.Variable(TypeUtils.DapperRowCollectionType, "InEntriesExParameter");

    /// <summary>
    ///     Gets the variable expression representing the enumerator for the input collection.
    ///     Used to iterate through the input Dapper rows.
    /// </summary>
    public ParameterExpression InEntriesExVariable { get; } =
        Expression.Variable(TypeUtils.DapperRowIteratorType, "InEntriesExVariable");

    /// <summary>
    ///     Gets the variable expression representing the output collection of entities.
    ///     Used to accumulate the results of the query.
    /// </summary>
    public ParameterExpression OutEntitiesExVariable { get; init; }

    /// <summary>
    ///     Gets the variable expression representing the current input row being processed.
    ///     Used to access the current Dapper row during iteration.
    /// </summary>
    public ParameterExpression CurrentEntryExVariable { get; } =
        Expression.Variable(TypeUtils.DapperRowType, "CurrentEntryExVariable");

    /// <summary>
    ///     Gets the variable expression representing the current output entity being constructed.
    ///     Used to build and populate the result entity for each row.
    /// </summary>
    public ParameterExpression CurrentEntityExVariable { get; init; }

    /// <summary>
    ///     Gets the variable expression used for indexing or referencing the current output entity.
    ///     Used in scenarios where indexed assignment or mapping is required.
    /// </summary>
    public ParameterExpression IndexerExVariable { get; init; }

    /// <summary>
    ///     Gets the list of expressions used for processing joined rows.
    ///     Each expression represents logic for mapping or initializing joined entities.
    /// </summary>
    public List<Expression> JoinRows { get; } = [];

    /// <summary>
    ///     Gets the block expression representing the complete set of operations for the query.
    ///     This block contains all variable declarations and expressions required to process the query.
    /// </summary>
    public BlockExpression Block { get; } = Expression.Block();
}
