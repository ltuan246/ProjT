namespace KISS.QueryBuilder.FluentBuilder.Builders.OrderByBuilders;

/// <summary>
///     An interface that defines the order by entry builder type.
/// </summary>
/// <typeparam name="TRecordset">The type in the recordset.</typeparam>
public interface IOrderByBuilderEntry<TRecordset> : IFluentSqlBuilder
{
    /// <summary>
    ///     Appends the <c>ORDER BY</c> clause to the builder.
    /// </summary>
    /// <returns>The <see cref="IOrderByBuilder{TRecordset}" /> instance.</returns>
    IOrderByBuilder<TRecordset> OrderBy();

    /// <summary>
    ///     Appends the <c>ORDER BY</c> clause to the builder.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IOrderByBuilder{TRecordset}" /> instance.</returns>
    IOrderByBuilder<TRecordset> OrderBy(bool condition);
}
