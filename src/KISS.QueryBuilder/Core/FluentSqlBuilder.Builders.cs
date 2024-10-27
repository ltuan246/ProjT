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
                    var assignProperty = Expression.Property(ReturnParam, memberExpression.Member.Name);
                    var assignExpression = Expression.Assign(assignProperty, relationParam);
                    BlockMapSequence.Add((relationParam, assignExpression));

                    // Define the expressions for setting Customer and merging OrderItems
                    var accAssignment = Expression.Assign(
                        Expression.Property(AccumulatedParam, memberExpression.Member.Name),
                        Expression.Property(CurrentParam, memberExpression.Member.Name));
                    BlockAggregateSequence.Add(accAssignment);

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
    public IJoinBuilder<TRecordset> InnerJoin(bool condition)
    {
        return this;
    }

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

        var map = CreatingMap();
        var query = CreatingQuery();
        var data = query.Invoke(null, [
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

        _ = data;

        // Combine expressions to form the body of the Aggregate lambda
        var block = Expression.Block([..BlockAggregateSequence, AccumulatedParam]);

        // Create the lambda expression (accumulatedOrder, currentOrder) => { ... }
        var aggregateLambda = Expression.Lambda<Func<TRecordset, TRecordset, TRecordset>>(
            block,
            AccumulatedParam,
            CurrentParam);

        // Compile and execute the lambda using Aggregate
        var aggregateFunc = aggregateLambda.Compile();

        var aggregateMethod = typeof(Enumerable)
            .GetMethods()
            .FirstOrDefault(m => m.Name == "Aggregate" && m.GetParameters().Length == 2)!
            .MakeGenericMethod(typeof(TRecordset));

        var queryAndAggregateResult = aggregateMethod.Invoke(null, [
            data!, // Result from Query
            aggregateFunc // Lambda for aggregation
        ]);

        _ = queryAndAggregateResult;

        return [];
    }
}
