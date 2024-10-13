namespace KISS.QueryBuilder.Core;

/// <summary>
///     Implements <see cref="IQueryBuilder" /> interfaces and <see cref="IQueryBuilderEntry" /> interfaces
///     for the <see cref="FluentSqlBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of results to return.</typeparam>
public sealed partial class FluentSqlBuilder<TEntity> : IQueryBuilder, IQueryBuilderEntry
{
    /// <inheritdoc />
    public string Sql
        => SqlBuilder.ToString();

    /// <inheritdoc />
    public DynamicParameters Parameters
        => SqlFormatter.Parameters;

    /// <inheritdoc />
    public ISelectBuilder Select<TRecordset, TResult>(Expression<Func<TRecordset, TResult>> columns)
        where TRecordset : TEntity
        => throw new NotImplementedException();

    /// <inheritdoc />
    public ISelectDistinctBuilder SelectDistinct() => throw new NotImplementedException();

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
    public IList<TResult> ToList<TResult>()
        where TResult : TEntity
        => throw new NotImplementedException();
}
