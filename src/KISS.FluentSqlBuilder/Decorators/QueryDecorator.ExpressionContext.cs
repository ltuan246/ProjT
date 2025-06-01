namespace KISS.FluentSqlBuilder.Decorators;

/// <summary>
///     Provides expression variables and context for building and executing SQL queries
///     in the QueryDecorator. This abstract base supplies access to expression parameters,
///     variables, and join logic required for constructing LINQ expression trees and
///     supporting advanced query scenarios.
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
