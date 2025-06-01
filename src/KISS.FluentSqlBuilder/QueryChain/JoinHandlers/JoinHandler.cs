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
/// <typeparam name="TReturn">
///     The combined type to return as the result of the join operation.
/// </typeparam>
/// <param name="LeftKeySelector">
///     An expression selecting the key from the left relation for the join condition.
///     Defines the left side of the join equality.
/// </param>
/// <param name="RightKeySelector">
///     An expression selecting the key from the right relation for the join condition.
///     Defines the right side of the join equality.
/// </param>
public abstract partial record JoinHandler<TRelation, TReturn>(
    Expression LeftKeySelector,
    Expression RightKeySelector) : QueryHandler(SqlStatement.Join, Expression.Equal(LeftKeySelector, RightKeySelector))
{
    /// <summary>
    ///     Gets the type of the relation (source table or entity) being joined.
    /// </summary>
    public Type RelationType { get; } = typeof(TRelation);

    /// <inheritdoc />
    protected override void Process()
    {
        // Ensures the composite query is wrapped with a JoinDecorator for join processing.
        if (Composite is not JoinDecorator)
        {
            Composite = new JoinDecorator(Composite);
        }

        // Generate the table alias and select clause for the joined relation.
        var alias = Composite.GetAliasMapping(RelationType);
        var sourceProperties = RelationType.GetProperties()
            .Where(p => p.CanWrite)
            .Select(p => $"{alias}.{p.Name} AS {alias}_{p.Name}")
            .ToList();

        // Add the generated select clause to the SQL statement collection.
        Composite.SqlStatements[SqlStatement.Select].Add(string.Join(", ", sourceProperties));
    }
}
