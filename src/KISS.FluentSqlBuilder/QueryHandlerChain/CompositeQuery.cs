namespace KISS.FluentSqlBuilder.QueryHandlerChain;

/// <summary>
///     CompositeQuery.
/// </summary>
public sealed partial class CompositeQuery(DbConnection connection)
{
    /// <summary>
    ///     The database connections.
    /// </summary>
    private DbConnection Connection { get; } = connection;

    /// <summary>
    ///     AddHandler.
    /// </summary>
    public void GetList()
    {
        _ = Connection.Query(Sql, Parameters)
            .Cast<IDictionary<string, object>>()
            .ToList();

        var dapperRowCollectionParameter = Expression.Parameter(typeof(IEnumerable<IDictionary<string, object>>), "dataCollection");
        var dapperRowStreamMethod = typeof(IEnumerable<IDictionary<string, object>>).GetMethod("GetEnumerator")!;

        var dapperRowStreamVariable = Expression.Variable(typeof(IEnumerator<IDictionary<string, object>>), "dataStream");
        var streamMoveNextMethod = typeof(IEnumerator).GetMethod("MoveNext")!;

        // Build while-loop
        MemberExpression currentDapperRow = Expression.Property(dapperRowStreamVariable, "Current");
        BlockExpression whileBody = Expression.Block(
            [IterationRowVariable],
            [Expression.Assign(IterationRowVariable, currentDapperRow)]);

        LabelTarget breakLabel = Expression.Label();
        LoopExpression whileLoop = Expression.Loop(
            Expression.IfThenElse(
                Expression.Call(dapperRowStreamVariable, streamMoveNextMethod),
                whileBody,
                Expression.Break(breakLabel)),
            breakLabel);

        BlockExpression fullBlock = Expression.Block(
            [dapperRowStreamVariable],
            [Expression.Assign(dapperRowStreamVariable, Expression.Call(dapperRowCollectionParameter, dapperRowStreamMethod)), whileLoop]);

        _ = Expression.Lambda(fullBlock, []).Compile();
    }
}
