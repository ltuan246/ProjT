namespace KISS.QueryBuilder.Builders.WhereBuilders;

/// <summary>
///     An interface for building SQL where clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface IWhereBuilder<TRecordset> : IGroupByBuilderEntry<TRecordset>
{
    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <returns>The <see cref="IWhereBuilder{TRecordset}" /> instance.</returns>
    IWhereBuilder<TRecordset> Where();

    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) with a condition.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IWhereBuilder{TRecordset}" /> instance.</returns>
    IWhereBuilder<TRecordset> Where(bool condition);
}
