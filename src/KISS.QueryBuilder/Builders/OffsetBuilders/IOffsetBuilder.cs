namespace KISS.QueryBuilder.Builders.OffsetBuilders;

/// <summary>
///     An interface for adding an OFFSET clause to the query.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface IOffsetBuilder<TRecordset> : IFluentSqlBuilder<TRecordset>
{
    /// <summary>
    ///     Appends the <c>OFFSET</c> clause to the query.
    /// </summary>
    /// <param name="offset">The number of rows to skip before starting to return rows.</param>
    /// <returns>The <see cref="IFluentSqlBuilder{TRecordset}" /> instance.</returns>
    IFluentSqlBuilder<TRecordset> Offset(int offset);
}
