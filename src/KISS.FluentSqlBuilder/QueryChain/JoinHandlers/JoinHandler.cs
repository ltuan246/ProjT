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
///     The combined type to return.
///     This type represents the result of the join operation.
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
public abstract partial record JoinHandler<TRelation, TReturn>(
    Expression LeftKeySelector,
    Expression RightKeySelector) : QueryHandler(SqlStatement.Join, Expression.Equal(LeftKeySelector, RightKeySelector)), IJoinHandler
{
    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    public Type OutDictEntityType { get; } = typeof(Dictionary<object, TReturn>);

    /// <summary>
    /// RelationType.
    /// </summary>
    public Type RelationType { get; } = typeof(TRelation);

    /// <summary>
    ///     Gets the CompositeQuery instance being processed by this handler.
    ///     This property provides access to the query being built and allows handlers
    ///     to modify the query state during processing.
    /// </summary>
    public new JoinDecorator<TRelation, TReturn> Composite { get; set; } = default!;

    /// <summary>
    /// JoinRowBlock.
    /// </summary>
    public Expression JoinRowBlock { get; set; } = Expression.Block();

    /// <inheritdoc />
    public override IComposite Handle(IComposite composite)
    {
        // Assigns the provided CompositeQuery to this handler for processing.
        Composite = new JoinDecorator<TRelation, TReturn>(composite);
        return base.Handle(Composite);
    }

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
