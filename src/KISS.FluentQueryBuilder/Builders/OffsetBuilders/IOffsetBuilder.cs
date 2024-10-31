namespace KISS.FluentQueryBuilder.Builders.OffsetBuilders;

/// <summary>
///     An interface that defines the offset builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IOffsetBuilder<TEntity>
{
    /// <summary>
    ///     Appends the <c>OFFSET</c> clause and the <paramref name="offset" /> to the builder.
    /// </summary>
    /// <param name="offset">The number of rows to skip.</param>
    /// <returns>The <see cref="IFluentBuilder{TEntity}" /> instance.</returns>
    IFluentBuilder<TEntity> Offset(int offset);
}
