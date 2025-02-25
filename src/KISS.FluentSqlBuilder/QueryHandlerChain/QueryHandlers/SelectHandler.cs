namespace KISS.FluentSqlBuilder.QueryHandlerChain.QueryHandlers;

/// <summary>
///     SelectHandler.
/// </summary>
/// <typeparam name="TSource">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record SelectHandler<TSource, TReturn> : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
    {
        Type sourceType = typeof(TReturn);
        Type retrieveType = typeof(TReturn);

        Composite.SourceParameter = Expression.Parameter(sourceType, "source");
        Composite.RetrieveVariable = Expression.Variable(retrieveType, "retrieve");

        var alias = Composite.GetAliasMapping(sourceType);
        var retrieveProperties = retrieveType.GetProperties().Where(p => p.CanWrite).Select(p => $"{alias}.{p.Name} AS {alias}_{p.Name}").ToList();
        Composite.SqlStatements[SqlStatement.Select].Add($"{string.Join(", ", retrieveProperties)}");
    }
}

/// <summary>
///     SelectHandler.
/// </summary>
/// <typeparam name="TSource">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record NewSelectHandler<TSource, TReturn>(Expression Selector) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process() { }
}
