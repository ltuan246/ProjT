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
        switch (selector.Body)
        {
            case MemberExpression memberExpression:
                SelectSpecificColumns.Add(memberExpression.Member.Name);
                break;

            case NewExpression newExpression:
                // Handle multiple fields or field aliases
                var selectList = newExpression.Members!
                    .Select(m => m.Name)
                    .Zip(newExpression.Arguments, (name, arg) =>
                    {
                        if (arg is MemberExpression memberArg)
                        {
                            return name == memberArg.Member.Name
                                ? memberArg.Member.Name
                                : $"{memberArg.Member.Name} AS {name}";
                        }

                        return name;
                    })
                    .ToArray();

                SelectSpecificColumns.AddRange(selectList);
                break;

            default:
                throw new NotSupportedException("Expression not supported.");
        }

        return this;
    }

    /// <inheritdoc />
    public ISelectBuilder<TEntity> SelectDistinct([NotNull] Expression<Func<TEntity, object>> selector)
    {
        HasDistinct = true;
        return Select(selector);
    }

    /// <inheritdoc />
    public IJoinBuilder<TEntity> Join<TRelation>(
        Expression<Func<TEntity, object>> leftKeySelector,
        Expression<Func<TRelation, object>> rightKeySelector)
    {
        return this;
    }

    /// <inheritdoc />
    public IJoinBuilder<TEntity> Join<TRelation>(
        bool condition,
        Expression<Func<TEntity, object>> leftKeySelector,
        Expression<Func<TRelation, object>> rightKeySelector)
        => condition ? Join(leftKeySelector, rightKeySelector) : this;

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
    {
        List<string> clauses = ["SELECT"];

        if (HasDistinct)
        {
            clauses.Add("DISTINCT");
        }

        clauses.Add(SelectSpecificColumns.Count != 0 ? string.Join(", ", SelectSpecificColumns) : "*");
        clauses.Add($"FROM {typeof(TEntity).Name}s AS {DefaultEntityAliasTemplate}");
        clauses.Add(Sql);

        var query = string.Join(" ", clauses);

        _ = Connection.Query<TEntity>(query, Parameters).ToList();

        return [];
    }
}
