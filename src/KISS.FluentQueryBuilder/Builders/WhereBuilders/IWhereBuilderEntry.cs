namespace KISS.FluentQueryBuilder.Builders.WhereBuilders;

/// <summary>
///     An interface that defines the where builder entry type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IWhereBuilderEntry<TEntity>
{
    /// <summary>
    ///     Appends the <c>WHERE</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="expression">The LambdaExpression.</param>
    /// <returns>The <see cref="IWhereBuilder{TEntity}" /> instance.</returns>
    IWhereBuilder<TEntity> Where(Expression<Func<TEntity, bool>> expression);
}
