namespace KISS.QueryBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface for building SQL select queries.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface ISelectBuilderEntry<TRecordset> : IFluentSqlBuilder<TRecordset>
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
