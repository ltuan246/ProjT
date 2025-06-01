namespace KISS.FluentSqlBuilder.Decorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public abstract partial record QueryDecorator
{
    /// <inheritdoc />
    public ParameterExpression InEntriesExParameter { get; } = Inner.InEntriesExParameter;

    /// <inheritdoc />
    public ParameterExpression InEntriesExVariable { get; } = Inner.InEntriesExVariable;

    /// <inheritdoc />
    public ParameterExpression OutEntitiesExVariable { get; } = Inner.OutEntitiesExVariable;

    /// <inheritdoc />
    public ParameterExpression CurrentEntryExVariable { get; } = Inner.CurrentEntryExVariable;

    /// <inheritdoc />
    public ParameterExpression CurrentEntityExVariable { get; } = Inner.CurrentEntityExVariable;

    /// <inheritdoc />
    public ParameterExpression IndexerExVariable { get; } = Inner.IndexerExVariable;

    /// <inheritdoc />
    public List<Expression> JoinRows { get; } = Inner.JoinRows;

    /// <inheritdoc />
    public virtual BlockExpression Block { get; } = Inner.Block;
}
