namespace KISS.FluentSqlBuilder.Builders.HavingBuilders;

/// <summary>
///     An interface for building <c>HAVING</c> clauses.
/// </summary>
public interface IHavingBuilder;

/// <summary>
///     An interface for building <c>HAVING</c> clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IHavingBuilder<TRecordset, TReturn> :
    IGroupSelectBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>HAVING</c> clause (or the AND clause, if a HAVING clause is present) to the query.
    /// </summary>
    /// <param name="aggregationType">The aggregation types used in queries for calculating summary statistics.</param>
    /// <param name="selector">The table columns.</param>
    /// <returns>The <see cref="IHavingBuilder{TRecordset, TReturn}" /> instance.</returns>
    IHavingBuilder<TRecordset, TReturn> Having(
        SqlAggregation aggregationType,
        Expression<Func<TRecordset, IComparable>> selector);
}
