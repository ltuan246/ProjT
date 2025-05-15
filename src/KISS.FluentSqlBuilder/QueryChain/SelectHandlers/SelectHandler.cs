namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     A handler for processing SELECT clauses in a query chain.
///     This class is responsible for generating SQL SELECT statements and mapping
///     database results to strongly-typed objects.
/// </summary>
/// <typeparam name="TSource">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record SelectHandler<TSource, TReturn>() : QueryHandler(SqlStatement.Select), ISelectHandler
{
    /// <summary>
    ///     Gets the type representing the database record set.
    ///     This type defines the structure of the data being queried from the database.
    /// </summary>
    public Type InEntityType { get; } = typeof(TSource);

    /// <summary>
    ///     Gets the CompositeQuery instance being processed by this handler.
    ///     This property provides access to the query being built and allows handlers
    ///     to modify the query state during processing.
    /// </summary>
    public new SelectDecorator<TSource, TReturn> Composite { get; set; } = default!;

    /// <inheritdoc />
    public override IComposite Handle(IComposite composite)
    {
        // Assigns the provided CompositeQuery to this handler for processing.
        Composite = new SelectDecorator<TSource, TReturn>(composite);
        return base.Handle(Composite);
    }

    /// <summary>
    ///     Processes the SELECT clause by generating SQL statements for selecting
    ///     columns from the source entity and mapping them to the return type.
    /// </summary>
    protected override void Process()
    {
        var alias = Composite.GetAliasMapping(InEntityType);
        var sourceProperties = InEntityType.GetProperties()
            .Where(p => p.CanWrite)
            .Select(p => $"{alias}.{p.Name} AS {alias}_{p.Name}")
            .ToList();

        Composite.SqlStatements[SqlStatement.Select].Add(string.Join(", ", sourceProperties));
    }
}
