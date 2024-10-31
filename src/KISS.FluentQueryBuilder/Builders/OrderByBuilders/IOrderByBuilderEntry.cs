namespace KISS.FluentQueryBuilder.Builders.OrderByBuilders;

/// <summary>
///     An interface that defines the order by entry builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IOrderByBuilderEntry<TEntity> : IToCollectionBuilderEntry<TEntity>
{
    /// <summary>
    ///     Appends the <c>ORDER BY</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="columns">The column(s) based on which the rows will be ordered.</param>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <returns>The <see cref="IOrderByBuilder{TEntity}" /> instance.</returns>
    IOrderByBuilder<TEntity> OrderBy<TResult>(Expression<Func<TEntity, TResult>> columns);
}
