namespace KISS.FluentQueryBuilder.Builders.LimitBuilders;

/// <summary>
///     An interface that defines the limit builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface ILimitBuilder<TEntity>
{
    /// <summary>
    ///     Appends the <c>LIMIT</c> clause and the <paramref name="rows" /> to the builder.
    /// </summary>
    /// <param name="rows">The number of rows to fetch.</param>
    /// <returns>The <see cref="IOffsetBuilder{TEntity}" /> instance.</returns>
    IFluentBuilder<TEntity> Limit(int rows);
}
