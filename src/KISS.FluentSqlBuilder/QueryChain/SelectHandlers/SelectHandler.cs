namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     A handler for processing SELECT clauses in a query chain.
///     This class is responsible for generating SQL SELECT statements and mapping
///     database results to strongly-typed objects.
/// </summary>
/// <typeparam name="TSource">
///     The type representing the database record set (source entity).
/// </typeparam>
/// <typeparam name="TReturn">
///     The combined type to return as the result of the SELECT operation.
/// </typeparam>
public sealed record SelectHandler<TSource, TReturn>() : QueryHandler(SqlStatement.Select)
{
    /// <inheritdoc />
    protected override void Process()
    {
        // Wraps the provided CompositeQuery with a SelectDecorator for SELECT clause processing.
        Composite = new SelectDecorator(Composite);

        // Generate the table alias and select clause for the source entity.
        var alias = Composite.GetAliasMapping(Composite.InEntityType);
        var sourceProperties = Composite.InEntityType.GetProperties()
            .Where(p => p.CanWrite)
            .Select(p => $"{alias}.{p.Name} AS {alias}_{p.Name}")
            .ToList();

        // Add the generated select clause to the SQL statement collection.
        Composite.SqlStatements[SqlStatement.Select].Add(string.Join(", ", sourceProperties));
    }
}
