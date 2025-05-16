namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     A handler for processing SELECT clauses in a query chain.
///     This class is responsible for generating SQL SELECT statements and mapping
///     database results to strongly-typed objects.
/// </summary>
/// <typeparam name="TSource">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record SelectHandler<TSource, TReturn>() : QueryHandler(SqlStatement.Select)
{
    /// <inheritdoc />
    protected override void Process()
    {
        // Assigns the provided CompositeQuery to this handler for processing.
        Composite = new SelectDecorator<TSource, TReturn>(Composite);

        var alias = Composite.GetAliasMapping(Composite.InEntityType);
        var sourceProperties = Composite.InEntityType.GetProperties()
            .Where(p => p.CanWrite)
            .Select(p => $"{alias}.{p.Name} AS {alias}_{p.Name}")
            .ToList();

        Composite.SqlStatements[SqlStatement.Select].Add(string.Join(", ", sourceProperties));
    }
}
