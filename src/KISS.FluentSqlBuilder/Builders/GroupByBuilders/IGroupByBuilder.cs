namespace KISS.FluentSqlBuilder.Builders.GroupByBuilders;

/// <summary>
///     A marker interface for building <c>GROUP BY</c> clauses. This interface serves
///     as the base for all group by builders in the query building process.
/// </summary>
public interface IGroupByBuilder;

/// <summary>
///     An interface for building <c>GROUP BY</c> clauses with support for additional
///     grouping keys and HAVING clauses. This interface enables the construction of
///     complex grouped queries with multiple grouping criteria and filtering conditions.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view being queried.</typeparam>
/// <typeparam name="TReturn">The type of the combined result set after grouping.</typeparam>
public interface IGroupByBuilder<TRecordset, TReturn> :
    IHavingBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Specifies an additional grouping key to refine the grouped data
    ///     after the initial <c>GROUP BY</c> clause in the query. This method enables
    ///     multi-level grouping of results.
    /// </summary>
    /// <param name="selector">An expression selecting an additional property to group by.</param>
    /// <returns>
    ///     The <see cref="IGroupByBuilder{TRecordset, TReturn}" /> instance, enabling
    ///     further grouping operations or the addition of HAVING clauses.
    /// </returns>
    IGroupByBuilder<TRecordset, TReturn> ThenBy(Expression<Func<TRecordset, IComparable>> selector);
}
