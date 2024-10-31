namespace KISS.FluentQueryBuilder.Builders.WhereBuilders;

/// <summary>
///     An interface that defines the where builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IWhereBuilder<TEntity> : IGroupByBuilder<TEntity>
{
    /// <summary>
    ///     Appends the <c>WHERE</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="condition">The condition that specifies the criteria for filtering the data.</param>
    /// <returns>The <see cref="IWhereBuilder{TEntity}" /> instance.</returns>
    IWhereBuilder<TEntity> Where(Expression<Func<TEntity, bool>> condition);
}
