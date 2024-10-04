namespace KISS.FluentQueryBuilder.Builders.JoinBuilders;

/// <summary>
///     An interface that defines the join builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IJoinBuilder<TEntity> : IWhereBuilder<TEntity>
{
    /// <summary>
    ///     Appends the <c>INNER JOIN</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="table">The table that want to join.</param>
    /// <param name="condition">The condition that specifies the criteria for filtering the joined data.</param>
    /// <typeparam name="TResult">The type of table that want to join.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TEntity}" /> instance.</returns>
    IJoinBuilder<TEntity> InnerJoin<TResult>(
        Expression<Func<TEntity, TResult>> table,
        Expression<Func<TEntity, bool>> condition);

    /// <summary>
    ///     Appends the <c>LEFT JOIN</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="table">The table that want to join.</param>
    /// <param name="condition">The condition that specifies the criteria for filtering the joined data.</param>
    /// <typeparam name="TResult">The type of table that want to join.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TEntity}" /> instance.</returns>
    IJoinBuilder<TEntity> LeftJoin<TResult>(
        Expression<Func<TEntity, TResult>> table,
        Expression<Func<TEntity, bool>> condition);

    /// <summary>
    ///     Appends the <c>RIGHT JOIN</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="table">The table that want to join.</param>
    /// <param name="condition">The condition that specifies the criteria for filtering the joined data.</param>
    /// <typeparam name="TResult">The type of table that want to join.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TEntity}" /> instance.</returns>
    IJoinBuilder<TEntity> RightJoin<TResult>(
        Expression<Func<TEntity, TResult>> table,
        Expression<Func<TEntity, bool>> condition);
}
