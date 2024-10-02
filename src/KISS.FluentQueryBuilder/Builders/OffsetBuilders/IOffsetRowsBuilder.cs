namespace KISS.FluentQueryBuilder.Builders.OffsetBuilders;

/// <summary>
///     An interface that defines the offset rows builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IOffsetRowsBuilder<TEntity>
{
    /// <summary>
    ///     Appends the <c>OFFSET</c> clause, the <paramref name="offset" />, and the <c>ROWS</c> clause to the builder.
    /// </summary>
    /// <param name="offset">The number of rows to skip.</param>
    /// <returns>The <see cref="IFetchBuilder{TEntity}" /> instance.</returns>
    IFetchBuilder<TEntity> OffsetRows(int offset);
}
