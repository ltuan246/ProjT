namespace KISS.FluentSqlBuilder.QueryChain.JoinHandlers;

/// <summary>
///     A handler for processing join operations in a query chain, linking two relations via key equality.
///     This class provides the base functionality for generating SQL JOIN statements and mapping
///     joined results to strongly-typed objects.
/// </summary>
/// <typeparam name="TRelation">
///     The type of the relation (source table or entity).
///     This type is used to generate proper table names and column mappings.
/// </typeparam>
/// <param name="LeftKeySelector">
///     An expression selecting the key from the left relation for the join condition.
///     This defines the left side of the join equality.
/// </param>
/// <param name="RightKeySelector">
///     An expression selecting the key from the right relation for the join condition.
///     This defines the right side of the join equality.
/// </param>
/// <param name="MapSelector">
///     An expression mapping the joined result into the output type.
///     This defines how the joined data should be mapped to the result object.
/// </param>
public abstract partial record JoinHandler<TRelation>(
    Expression LeftKeySelector,
    Expression RightKeySelector,
    Expression MapSelector) : QueryHandler(SqlStatement.Join, Expression.Equal(LeftKeySelector, RightKeySelector))
{
    private Type RelationType { get; } = typeof(TRelation);

    /// <summary>
    ///     Processes the JOIN operation by generating the SQL JOIN statement
    ///     and setting up the result mapping.
    /// </summary>
    protected override void Process()
    {
        var alias = Composite.GetAliasMapping(RelationType);
        var sourceProperties = RelationType.GetProperties()
            .Where(p => p.CanWrite)
            .Select(p => $"{alias}.{p.Name} AS {alias}_{p.Name}")
            .ToList();

        Composite.SqlStatements[SqlStatement.Select].Add(string.Join(", ", sourceProperties));
    }
}
