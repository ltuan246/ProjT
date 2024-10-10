using System;

namespace KISS.QueryBuilder.Core;

/// <summary>
///     Implements <see cref="IQueryBuilder" /> interfaces for the <see cref="FluentSqlBuilder" /> type.
/// </summary>
internal sealed partial class FluentSqlBuilder : IQueryBuilder
{
    /// <inheritdoc />
    public string Sql => SqlBuilder.ToString();

    /// <inheritdoc />
    public ISelectBuilder Select() => throw new NotImplementedException();

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
}
