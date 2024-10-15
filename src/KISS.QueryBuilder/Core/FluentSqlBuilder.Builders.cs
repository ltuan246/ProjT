namespace KISS.QueryBuilder.Core;

/// <summary>
///     Implements <see cref="IQueryBuilder{TEntity}" /> and <see cref="IQueryBuilderEntry{TEntity}" />
///     for the <see cref="FluentSqlBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of results to return.</typeparam>
public sealed partial class FluentSqlBuilder<TEntity> : IQueryBuilder<TEntity>, IQueryBuilderEntry<TEntity>
{
    /// <inheritdoc />
    public string Sql
        => SqlBuilder.ToString();

    /// <inheritdoc />
    public DynamicParameters Parameters
        => SqlFormat.Parameters;

    /// <inheritdoc />
    public ISelectBuilder Select([NotNull] Expression<Func<TEntity, object>> columns)
    {
        var entity = typeof(TEntity);
        var table = entity.Name;
        var properties = columns.Parameters[1].GetType().GetProperties();
        var cols = properties.Select(p => $"[{p.Name}]").ToArray();

        SqlBuilder.Clear();
        Append($"SELECT {cols} FROM {table}s ");

        return this;
    }

    /// <inheritdoc />
    public ISelectDistinctBuilder SelectDistinct([NotNull] Expression<Func<TEntity, object>> columns)
    {
        var entity = typeof(TEntity);
        var table = entity.Name;
        var properties = columns.Parameters[1].GetType().GetProperties();
        var cols = properties.Select(p => $"[{p.Name}]").ToArray();

        SqlBuilder.Clear();
        Append($"SELECT DISTINCT {cols} FROM {table}s ");

        return this;
    }

    /// <inheritdoc />
    public IJoinBuilder InnerJoin() => throw new NotImplementedException();

    /// <inheritdoc />
    public IJoinBuilder InnerJoin(bool condition) => throw new NotImplementedException();

    /// <inheritdoc />
    public IJoinBuilder LeftJoin() => throw new NotImplementedException();

    /// <inheritdoc />
    public IJoinBuilder LeftJoin(bool condition) => throw new NotImplementedException();

    /// <inheritdoc />
    public IJoinBuilder RightJoin() => throw new NotImplementedException();

    /// <inheritdoc />
    public IJoinBuilder RightJoin(bool condition) => throw new NotImplementedException();

    /// <inheritdoc />
    public IWhereBuilder Where() => throw new NotImplementedException();

    /// <inheritdoc />
    public IWhereBuilder Where(bool condition) => throw new NotImplementedException();

    /// <inheritdoc />
    public IGroupByBuilder GroupBy() => throw new NotImplementedException();

    /// <inheritdoc />
    public IGroupByBuilder GroupBy(bool condition) => throw new NotImplementedException();

    /// <inheritdoc />
    public IHavingBuilder Having() => throw new NotImplementedException();

    /// <inheritdoc />
    public IHavingBuilder Having(bool condition) => throw new NotImplementedException();

    /// <inheritdoc />
    public IOrderByBuilder OrderBy() => throw new NotImplementedException();

    /// <inheritdoc />
    public IOrderByBuilder OrderBy(bool condition) => throw new NotImplementedException();

    /// <inheritdoc />
    public IFluentSqlBuilder Offset(int offset) => throw new NotImplementedException();

    /// <inheritdoc />
    public IOffsetBuilder Limit(int rows) => throw new NotImplementedException();

    /// <inheritdoc />
    public IFetchBuilder OffsetRows(int offset) => throw new NotImplementedException();

    /// <inheritdoc />
    public IFluentSqlBuilder FetchNext(int rows) => throw new NotImplementedException();

    /// <inheritdoc />
    public IList<TEntity> ToList()
        => throw new NotImplementedException();
}
