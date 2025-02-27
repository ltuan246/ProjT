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
        Type sourceType = typeof(TSource);
        Type retrieveType = typeof(TReturn);

        var alias = Composite.GetAliasMapping(sourceType);
        var sourceProperties = sourceType.GetProperties().Where(p => p.CanWrite).Select(p => $"{alias}.{p.Name} AS {alias}_{p.Name}").ToList();
        Composite.SqlStatements[SqlStatement.Select].Add($"{string.Join(", ", sourceProperties)}");

        Expression Init((ParameterExpression IterRowParameter, ParameterExpression CurrentEntityVariable) p) => Expression.Block(
            [p.CurrentEntityVariable],
            Expression.Assign(
                p.CurrentEntityVariable,
                Expression.MemberInit(
                    Expression.New(typeof(TReturn)),
                    Composite.CreateIterRowBindings(p.IterRowParameter, sourceType, retrieveType))));

        Composite.IterRowProcessors.Add(Init);
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
