namespace KISS.FluentQueryBuilder.Builders;

/// <summary>
///     A class that defines the fluent SQL builder type. The core <see cref="FluentBuilder{TEntity}" /> partial class.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public sealed partial class FluentBuilder<TEntity> : IFluentBuilder<TEntity>, IFluentBuilderEntry<TEntity>
{
    /// <inheritdoc />
    public ISelectDistinctBuilder<TEntity> SelectDistinct<TResult>(
        [NotNull] Expression<Func<TEntity, TResult>> columns)
    {
        Append(" SELECT DISTINCT ");
        Translate(columns.Body);
        Append($" FROM {typeof(TEntity).Name}s ");

        return this;
    }

    /// <inheritdoc />
    public IJoinBuilder<TEntity> InnerJoin<TResult>(
        Expression<Func<TEntity, TResult>> table,
        Expression<Func<TEntity, bool>> condition)
    {
        Append(" INNER JOIN ");
        Translate(table.Body);

        Append(" ON ");
        Translate(condition.Body);

        return this;
    }

    /// <inheritdoc />
    public IJoinBuilder<TEntity> LeftJoin<TResult>(
        Expression<Func<TEntity, TResult>> table,
        Expression<Func<TEntity, bool>> condition)
    {
        Append(" LEFT JOIN ");
        Translate(table.Body);

        Append(" ON ");
        Translate(condition.Body);

        return this;
    }

    /// <inheritdoc />
    public IJoinBuilder<TEntity> RightJoin<TResult>(
        Expression<Func<TEntity, TResult>> table,
        Expression<Func<TEntity, bool>> condition)
    {
        Append(" RIGHT JOIN ");
        Translate(table.Body);

        Append(" ON ");
        Translate(condition.Body);

        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TEntity> Where([NotNull] Expression<Func<TEntity, bool>> condition)
    {
        if (string.IsNullOrEmpty(Sql))
        {
            var entity = typeof(TEntity);
            var table = entity.Name;
            var propsName = entity.GetProperties().Select(p => $"[{p.Name}]").ToArray();
            var columns = string.Join(", ", propsName);
            Append($"SELECT {columns} FROM {table}s ");
        }

        Append(" WHERE ");
        Translate(condition.Body);

        return this;
    }

    /// <inheritdoc />
    public IGroupByBuilder<TEntity> GroupBy(Expression<Func<TEntity, bool>> columns)
    {
        Append(" GROUP BY ");
        Translate(columns.Body);

        return this;
    }

    /// <inheritdoc />
    public IHavingBuilder<TEntity> Having(Expression<Func<TEntity, bool>> condition)
    {
        Append(" HAVING ");
        Translate(condition.Body);

        return this;
    }

    /// <inheritdoc />
    public IOrderByBuilder<TEntity> OrderBy<TResult>(Expression<Func<TEntity, TResult>> columns)
    {
        Append(" ORDER BY ");
        Translate(columns.Body);

        return this;
    }

    /// <inheritdoc />
    public IFluentBuilder<TEntity> Offset(int offset)
    {
        Append($" OFFSET {offset} ");
        return this;
    }

    /// <inheritdoc />
    public IFluentBuilder<TEntity> Limit(int rows)
    {
        Append($" LIMIT {rows} ");
        return this;
    }

    /// <inheritdoc />
    public IFetchBuilder<TEntity> OffsetRows(int offset)
    {
        Append($" OFFSET {offset} ROWS ");
        return this;
    }

    /// <inheritdoc />
    public IFluentBuilder<TEntity> FetchNext(int rows)
    {
        Append($" FETCH NEXT {rows} ROWS ONLY ");
        return this;
    }

    /// <inheritdoc />
    public IList<TEntity> ToList()
        => Connection.Query<TEntity>(Sql, Parameters).ToList();

    /// <inheritdoc />
    public ISelectBuilder<TEntity> Select<TResult>([NotNull] Expression<Func<TEntity, TResult>> columns)
    {
        Append(" SELECT ");
        Translate(columns.Body);
        Append($" FROM {typeof(TEntity).Name}s ");

        return this;
    }
}
