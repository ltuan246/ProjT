namespace KISS.FluentQueryBuilder.Builders.GroupByBuilders;

/// <summary>
///     An interface that defines the group by builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IGroupByBuilder<TEntity> : IHavingBuilder<TEntity>
{
    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="columns">The column(s) based on which the rows will be grouped.</param>
    /// <returns>The <see cref="IHavingBuilder{TEntity}" /> instance.</returns>
    IGroupByBuilder<TEntity> GroupBy(Expression<Func<TEntity, bool>> columns);
}
