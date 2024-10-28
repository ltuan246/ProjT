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
        SqlBuilder.Clear();

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
    public ISelectBuilder<TRecordset> SelectDistinct(Expression<Func<TRecordset, object>> selector)
    {
        HasDistinct = true;
        return Select(selector);
    }

    /// <inheritdoc/>
    public IJoinBuilder<TRecordset> InnerJoin<TRelation, TKey>(
        Expression<Func<TRecordset, TRelation>> mapSelector,
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>
    {
        switch (mapSelector.Body)
        {
            case MemberExpression memberExpression:
                {
                    var relationParam = Expression.Parameter(typeof(TRelation), GetTableAlias(typeof(TRelation)));
                    var currentProperty = Expression.Property(ReturnParam, memberExpression.Member.Name);
                    var assignExpression = Expression.Assign(currentProperty, relationParam);
                    BlockMapSequence.Add((relationParam, assignExpression));

                    break;
                }

            default:
                throw new NotSupportedException("Expression not supported.");
        }

        Append(ClauseConstants.Join);
        AppendTableAlias(typeof(TRelation));
        Append(ClauseConstants.OnSeparator);

        Translate(leftKeySelector.Body);
        Append(BinaryOperandMap[ExpressionType.Equal]);
        Translate(rightKeySelector.Body);

        return this;
    }

    /// <inheritdoc/>
    public IJoinBuilder<TRecordset> InnerJoin<TRelation, TKey>(
        Expression<Func<TRecordset, List<TRelation>>> mapSelector,
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>
    {
        switch (mapSelector.Body)
        {
            case MemberExpression memberExpression:
                {
                    var relationParam = Expression.Parameter(typeof(TRelation), GetTableAlias(typeof(TRelation)));
                    var currentProperty = Expression.Property(ReturnParam, memberExpression.Member.Name);

                    var newList = Expression.IfThen(
                        Expression.Equal(currentProperty, Expression.Constant(null)),
                        Expression.Assign(currentProperty, Expression.New(typeof(List<TRelation>))));

                    // Add relation to mapSelector if it exists
                    var addToList = Expression.IfThen(
                        Expression.NotEqual(relationParam, Expression.Constant(null)),
                        Expression.Block(
                            newList,
                            Expression.Call(currentProperty, "Add", null, relationParam)));

                    BlockMapSequence.Add((relationParam, addToList));

                    break;
                }

            default:
                throw new NotSupportedException("Expression not supported.");
        }

        Append(ClauseConstants.Join);
        AppendTableAlias(typeof(TRelation));
        Append(ClauseConstants.OnSeparator);

        Translate(leftKeySelector.Body);
        Append(BinaryOperandMap[ExpressionType.Equal]);
        Translate(rightKeySelector.Body);

        return this;
    }

    /// <inheritdoc/>
    public IJoinBuilder<TRecordset> InnerJoin<TRelation, TKey>(
        bool condition,
        Expression<Func<TRecordset, TRelation>> mapSelector,
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>
        => condition ? InnerJoin(mapSelector, leftKeySelector, rightKeySelector) : this;

    /// <inheritdoc/>
    public IJoinBuilder<TRecordset> InnerJoin<TRelation, TKey>(
        bool condition,
        Expression<Func<TRecordset, List<TRelation>>> mapSelector,
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>
        => condition ? InnerJoin(mapSelector, leftKeySelector, rightKeySelector) : this;

    /// <inheritdoc/>
    public IWhereBuilder<TRecordset> Where(Expression<Func<TRecordset, bool>> predicate)
    {
        if (ClauseActions.Contains(ClauseAction.Where))
        {
            Append(ClauseConstants.AndSeparator);
        }
        else
        {
            ClauseActions.Add(ClauseAction.Where);
            Append(ClauseConstants.Where);
        }

        Translate(predicate.Body);
        return this;
    }

    /// <inheritdoc/>
    public IWhereBuilder<TRecordset> Where(bool condition, Expression<Func<TRecordset, bool>> predicate)
        => condition ? Where(predicate) : this;

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
    public IOrderByBuilder<TRecordset> OrderBy(Expression<Func<TRecordset, object>> selector)
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
        if (BlockMapSequence.Count == 0)
        {
            return Connection.Query<TRecordset>(Sql, Parameters).ToList();
        }

        Dictionary<string, TRecordset> dict = [];
        var map = BuildMapRecordset(dict);
        var query = CreatingQuery();
        _ = query.Invoke(null, [
            Connection,
            Sql, // SQL query string
            map, // Mapping function
            Parameters, // Dapper DynamicParameters
            null, // IDbTransaction, set to null
            true, // Buffered, true by default
            "Id", // SplitOn, default to "Id"
            null, // CommandTimeout, null
            null // CommandType, null
        ]);

        return dict.Values.ToList();
    }
}
