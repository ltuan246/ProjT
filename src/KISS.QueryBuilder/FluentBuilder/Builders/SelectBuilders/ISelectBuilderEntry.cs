namespace KISS.QueryBuilder.FluentBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface that defines the select builder entry type.
/// </summary>
/// <typeparam name="TRecordset">The type in the recordset.</typeparam>
public interface ISelectBuilderEntry<TRecordset> : IFluentSqlBuilder
{
    /// <summary>
    ///     Appends the <c>SELECT</c> clause to the builder.
    /// </summary>
    /// <param name="selector">The table columns.</param>
    /// <returns>The <see cref="ISelectBuilder{TRecordset}" /> instance.</returns>
    ISelectBuilder<TRecordset> Select(Expression<Func<TRecordset, object>> selector);

    /// <summary>
    ///     Appends the <c>SELECT DISTINCT</c> clause to the builder.
    /// </summary>
    /// <param name="selector">The table columns.</param>
    /// <returns>The <see cref="ISelectBuilder{TRecordset}" /> instance.</returns>
    ISelectBuilder<TRecordset> SelectDistinct(Expression<Func<TRecordset, object>> selector);
}
