namespace KISS.QueryBuilder.FluentBuilder.Builders.GroupByBuilders;

/// <summary>
///     An interface that defines the group by builder type.
/// </summary>
/// <typeparam name="TRecordset">The type in the recordset.</typeparam>
public interface IGroupByBuilder<TRecordset> : IHavingBuilder<TRecordset>
{
    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause to the builder.
    /// </summary>
    /// <returns>The <see cref="IGroupByBuilder{TRecordset}" /> instance.</returns>
    IGroupByBuilder<TRecordset> GroupBy();

    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause to the builder.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IGroupByBuilder{TRecordset}" /> instance.</returns>
    IGroupByBuilder<TRecordset> GroupBy(bool condition);
}
