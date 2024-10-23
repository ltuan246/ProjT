namespace KISS.QueryBuilder.Core;

/// <summary>
///     Implements <see cref="IQueryBuilder{TEntity}" /> and <see cref="IQueryBuilderEntry{TEntity}" />
///     for the <see cref="FluentSqlBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of results to return.</typeparam>
public sealed partial class FluentSqlBuilder<TEntity> : IQueryBuilder<TEntity>, IQueryBuilderEntry<TEntity>
{
    /// <inheritdoc />
    public ISelectBuilder<TEntity> Select([NotNull] Expression<Func<TEntity, object>> selector)
    {
        Append("SELECT");

        if (HasDistinct)
        {
            Append("DISTINCT");
        }

        switch (selector.Body)
        {
            case MemberExpression memberExpression:
                Translate(memberExpression);
                break;

            case NewExpression newExpression:
                Translate(newExpression);
                break;

            default:
                throw new NotSupportedException("Expression not supported.");
        }

        string tableAlias = GetTableAlias(typeof(TEntity));
        Append($"FROM {typeof(TEntity).Name}s {tableAlias}");

        return this;
    }

    /// <inheritdoc />
    public ISelectBuilder<TEntity> SelectDistinct([NotNull] Expression<Func<TEntity, object>> selector)
    {
        HasDistinct = true;
        return Select(selector);
    }

    /// <inheritdoc />
    public IJoinBuilder<TEntity> Join<TRelation, TKey>(
        [NotNull] Expression<Func<TEntity, TRelation>> resultSelector,
        [NotNull] Expression<Func<TEntity, TKey>> leftKeySelector,
        [NotNull] Expression<Func<TRelation, TKey>> rightKeySelector)
    {
        string tableAlias = GetTableAlias(typeof(TRelation));

        Append($"JOIN {typeof(TRelation).Name}s {tableAlias} ON");
        Translate(leftKeySelector.Body);
        Append("=");
        Translate(rightKeySelector.Body);

        return this;
    }

    /// <inheritdoc />
    public IJoinBuilder<TEntity> Join<TRelation, TKey>(
        bool condition,
        Expression<Func<TEntity, TRelation>> resultSelector,
        Expression<Func<TEntity, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        => condition ? Join(resultSelector, leftKeySelector, rightKeySelector) : this;

    /// <inheritdoc />
    public IWhereBuilder<TEntity> Where() => throw new NotImplementedException();

    /// <inheritdoc />
    public IWhereBuilder<TEntity> Where(bool condition) => throw new NotImplementedException();

    /// <inheritdoc />
    public IGroupByBuilder<TEntity> GroupBy() => throw new NotImplementedException();

    /// <inheritdoc />
    public IGroupByBuilder<TEntity> GroupBy(bool condition) => throw new NotImplementedException();

    /// <inheritdoc />
    public IHavingBuilder<TEntity> Having() => throw new NotImplementedException();

    /// <inheritdoc />
    public IHavingBuilder<TEntity> Having(bool condition) => throw new NotImplementedException();

    /// <inheritdoc />
    public IOrderByBuilder<TEntity> OrderBy() => throw new NotImplementedException();

    /// <inheritdoc />
    public IOrderByBuilder<TEntity> OrderBy(bool condition) => throw new NotImplementedException();

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
    {
        _ = Sql;
        // _ = Connection.Query<TEntity>(Sql, Parameters).ToList();

        return [];
    }
}
