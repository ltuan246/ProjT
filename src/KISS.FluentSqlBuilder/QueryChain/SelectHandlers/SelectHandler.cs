namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     A handler for processing <c>SELECT</c> in a query chain.
/// </summary>
/// <typeparam name="TSource">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record SelectHandler<TSource, TReturn> : QueryHandler
{
    /// <summary>
    ///     The type representing the database record set.
    /// </summary>
    private Type SourceEntity { get; } = typeof(TSource);

    /// <summary>
    ///     The combined type to return.
    /// </summary>
    private Type RetrieveEntity { get; } = typeof(TReturn);

    /// <inheritdoc />
    protected override void Process()
    {
        var alias = Composite.GetAliasMapping(SourceEntity);
        var sourceProperties = SourceEntity.GetProperties()
            .Where(p => p.CanWrite)
            .Select(p => $"{alias}.{p.Name} AS {alias}_{p.Name}")
            .ToList();

        Composite.SqlStatements[SqlStatement.Select].Add($"{string.Join(", ", sourceProperties)}");
    }

    /// <inheritdoc />
    protected override void BuildExpression()
    {
        Expression Init((ParameterExpression IterRowParameter, ParameterExpression CurrentEntityVariable) p)
        {
            return Expression.Block(
                Expression.Assign(
                    p.CurrentEntityVariable,
                    Expression.MemberInit(
                        Expression.New(typeof(TReturn)),
                        Composite.CreateIterRowBindings(p.IterRowParameter, SourceEntity, RetrieveEntity))));
        }

        Composite.IterRowProcessor = Init;
    }
}
