namespace KISS.QueryBuilder.FluentBuilder.Builders.HavingBuilders;

/// <summary>
///     An interface that defines the having builder type.
/// </summary>
/// <typeparam name="TRecordset">The type in the recordset.</typeparam>
public interface IHavingBuilder<TRecordset> : IOrderByBuilderEntry<TRecordset>
{
    /// <summary>
    ///     Appends the <c>HAVING</c> clause to the builder.
    /// </summary>
    /// <returns>The <see cref="IHavingBuilder{TRecordset}" /> instance.</returns>
    IHavingBuilder<TRecordset> Having();

    /// <summary>
    ///     Appends the <c>HAVING</c> clause to the builder.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IHavingBuilder{TRecordset}" /> instance.</returns>
    IHavingBuilder<TRecordset> Having(bool condition);
}
