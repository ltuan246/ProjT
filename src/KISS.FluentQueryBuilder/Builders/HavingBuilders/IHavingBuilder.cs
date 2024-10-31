namespace KISS.FluentQueryBuilder.Builders.HavingBuilders;

/// <summary>
///     An interface that defines the having builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IHavingBuilder<TEntity> : IOrderByBuilderEntry<TEntity>
{
    /// <summary>
    ///     Appends the <c>HAVING</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="condition">The condition that compares the column to certain conditions that require filtering.</param>
    /// <returns>The <see cref="IHavingBuilder{TEntity}" /> instance.</returns>
    IHavingBuilder<TEntity> Having(Expression<Func<TEntity, bool>> condition);
}
