namespace KISS.QueryBuilder.Core;

/// <summary>
///     Contains the builder methods for different SQL clauses, which is probably how the query is constructed.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public partial class FluentSqlBuilder<TRecordset> : IQueryBuilder<TRecordset>
{
    /// <inheritdoc/>
    public ISelectBuilder<TRecordset> Select(Expression<Func<TRecordset, object>> selector)
    {
        Append(ClauseConstants.Select);

        if (HasDistinct)
        {
            Append(ClauseConstants.Distinct);
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

        Append(ClauseConstants.From);
        AppendTableAlias(RootTable);

        return this;
    }

    /// <inheritdoc/>
    public ISelectBuilder<TRecordset> SelectDistinct()
    {
        HasDistinct = true;
        return this;
    }

    /// <inheritdoc/>
    public IJoinBuilder<TRecordset> InnerJoin()
    {
        return this;
    }

    /// <inheritdoc/>
    public IJoinBuilder<TRecordset> InnerJoin(bool condition)
    {
        return this;
    }

    /// <inheritdoc/>
    public IWhereBuilder<TRecordset> Where()
    {
        return this;
    }

    /// <inheritdoc/>
    public IWhereBuilder<TRecordset> Where(bool condition)
    {
        return this;
    }

    /// <inheritdoc/>
    public IGroupByBuilder<TRecordset> GroupBy()
    {
        return this;
    }

    /// <inheritdoc/>
    public IGroupByBuilder<TRecordset> GroupBy(bool condition)
    {
        return this;
    }

    /// <inheritdoc/>
    public IHavingBuilder<TRecordset> Having()
    {
        return this;
    }

    /// <inheritdoc/>
    public IHavingBuilder<TRecordset> Having(bool condition)
    {
        return this;
    }

    /// <inheritdoc/>
    public IOrderByBuilder<TRecordset> OrderBy()
    {
        return this;
    }

    /// <inheritdoc/>
    public IOrderByBuilder<TRecordset> OrderBy(bool condition)
    {
        return this;
    }

    /// <inheritdoc/>
    public IOffsetBuilder<TRecordset> Limit(int rows)
    {
        return this;
    }

    /// <inheritdoc/>
    public IFluentSqlBuilder<TRecordset> Offset(int offset)
    {
        return this;
    }

    /// <inheritdoc/>
    public List<TRecordset> ToList()
    {
        return [];
    }
}
