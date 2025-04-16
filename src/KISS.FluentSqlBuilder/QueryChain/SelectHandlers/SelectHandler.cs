namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     A handler for processing SELECT clauses in a query chain.
///     This class is responsible for generating SQL SELECT statements and mapping
///     database results to strongly-typed objects.
/// </summary>
/// <typeparam name="TSource">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record SelectHandler<TSource, TReturn>() : QueryHandler(Expression.Block(), SqlStatement.Select)
{
    /// <summary>
    ///     Gets the type representing the database record set.
    ///     This type defines the structure of the data being queried from the database.
    /// </summary>
    private Type SourceEntity { get; } = typeof(TSource);

    /// <summary>
    ///     Gets the combined type to return.
    ///     This type defines the structure of the object that will be created
    ///     from the query results.
    /// </summary>
    private Type RetrieveEntity { get; } = typeof(TReturn);

    /// <summary>
    ///     Processes the SELECT clause by generating SQL statements for selecting
    ///     columns from the source entity and mapping them to the return type.
    /// </summary>
    protected override void TranslateExpression()
    {
        var alias = Composite.GetAliasMapping(SourceEntity);
        var sourceProperties = SourceEntity.GetProperties()
            .Where(p => p.CanWrite)
            .Select(p => $"{alias}.{p.Name} AS {alias}_{p.Name}")
            .ToList();

        Composite.SqlStatements[SqlStatement.Select].Add($"{string.Join(", ", sourceProperties)}");
    }

    /// <summary>
    ///     Builds the expression for processing query results and mapping them
    ///     to the return type. This method creates the necessary expression tree
    ///     for converting database rows into strongly-typed objects.
    /// </summary>
    protected override void ExpressionIntegration()
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
