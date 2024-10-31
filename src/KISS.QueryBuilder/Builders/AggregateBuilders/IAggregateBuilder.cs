namespace KISS.QueryBuilder.Builders.AggregateBuilders;

/// <summary>
///     Adds an aggregate function to the SQL query being built.
///     Used in <see cref="IGroupByBuilderEntry{TRecordset}" /> queries,
///     the aggregate function applies to a specific column and optionally assigns an alias for the aggregated result.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface IAggregateBuilder<TRecordset> : IHavingBuilder<TRecordset>
{
    /// <summary>
    ///     Appends the <c>THE AGGREGATE FUNCTION</c> to a specific column.
    /// </summary>
    /// <returns>The <see cref="IAggregateBuilder{TRecordset}" /> instance.</returns>
    IAggregateBuilder<TRecordset> Aggregate();
}
