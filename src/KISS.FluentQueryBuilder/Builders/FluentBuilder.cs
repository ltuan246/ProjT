namespace KISS.FluentQueryBuilder.Builders;

public sealed partial record FluentBuilder<TEntity>(DbConnection DbConnection)
    : IFluentBuilder<TEntity>, IFluentBuilderEntry<TEntity>
{
    /// <inheritdoc />
    public ISelectDistinctBuilder SelectDistinct([NotNull] LambdaExpression expression)
    {
        SetEntryClause(ClauseAction.SelectDistinct, expression.Body);
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TEntity> Where([NotNull] Expression<Func<TEntity, bool>> expression)
    {
        SetEntryClause(ClauseAction.Where, expression.Body);
        return this;
    }

    /// <inheritdoc />
    public IList<TEntity> ToList()
    {
        if (EntryClause.TryGetValue(ClauseAction.Select, out var selectExpression))
        {
            Append(" SELECT ");
            Visit(selectExpression);
            Append($" FROM {typeof(TEntity).Name}s ");
        }
        else if (EntryClause.TryGetValue(ClauseAction.SelectDistinct, out var selectDistinctExpression))
        {
            Append(" SELECT DISTINCT ");
            Visit(selectDistinctExpression);
            Append($" FROM {typeof(TEntity).Name}s ");
        }
        else
        {
            var entity = typeof(TEntity);
            var table = entity.Name;
            var propsName = entity.GetProperties().Select(p => $"[{p.Name}]").ToArray();
            var columns = string.Join(", ", propsName);
            Append($"SELECT {columns} FROM {table}s ");
        }

        if (EntryClause.TryGetValue(ClauseAction.Where, out var whereExpression))
        {
            Append(" WHERE ");
            Visit(whereExpression);
        }

        return DbConnection.Query<TEntity>(Sql, Parameters).ToList();
    }

    /// <inheritdoc />
    public ISelectBuilder<TEntity> Select<TResult>([NotNull] Expression<Func<TEntity, TResult>> expression)
    {
        SetEntryClause(ClauseAction.Select, expression.Body);
        return this;
    }
}
