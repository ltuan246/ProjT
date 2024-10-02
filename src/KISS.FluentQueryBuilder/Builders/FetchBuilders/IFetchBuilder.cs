namespace KISS.FluentQueryBuilder.Builders.FetchBuilders;

/// <summary>
///     An interface that defines the fetch builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IFetchBuilder<TEntity>
{
    /// <summary>
    ///     Appends the <c>FETCH NEXT</c> clause, the <paramref name="rows" />, and the <c>ROWS ONLY</c> clause to the builder.
    /// </summary>
    /// <param name="rows">The number of rows to fetch.</param>
    /// <returns>The <see cref="IFluentBuilder{TEntity}" /> instance.</returns>
    IFluentBuilder<TEntity> FetchNext(int rows);
}
