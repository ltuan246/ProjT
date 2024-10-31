namespace KISS.QueryBuilder.Builders.LimitBuilders;

/// <summary>
///     An interface for adding a LIMIT clause to the query.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface ILimitBuilder<TRecordset> : IFluentSqlBuilder<TRecordset>
{
    /// <summary>
    ///     Appends the <c>LIMIT</c> clause to the query.
    /// </summary>
    /// <param name="rows">The maximum number of rows to return.</param>
    /// <returns>The <see cref="IOffsetBuilder{TRecordset}" /> instance.</returns>
    IOffsetBuilder<TRecordset> Limit(int rows);
}
