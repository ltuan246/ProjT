namespace KISS.QueryBuilder.FluentBuilder.Builders.WhereBuilders;

/// <summary>
///     An interface that defines the where builder type.
/// </summary>
/// <typeparam name="TRecordset">The type in the recordset.</typeparam>
public interface IWhereBuilder<TRecordset> : IGroupByBuilder<TRecordset>
{
    /// <summary>
    ///     Appends the <c>WHERE</c> clause to the builder.
    ///     If the <c>WHERE</c> clause is already present, the <c>AND</c> clause is appended.
    /// </summary>
    /// <returns>The <see cref="IWhereBuilder{TRecordset}" /> instance.</returns>
    IWhereBuilder<TRecordset> Where();

    /// <summary>
    ///     Appends the <c>WHERE</c> clause to the builder.
    ///     If the <c>WHERE</c> clause is already present, the <c>AND</c> clause is appended.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IWhereBuilder{TRecordset}" /> instance.</returns>
    IWhereBuilder<TRecordset> Where(bool condition);
}
