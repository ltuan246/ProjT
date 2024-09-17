namespace KISS.FluentQueryBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface that defines the select distinct builder entry type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface ISelectDistinctBuilderEntry<TEntity>
{
    /// <summary>
    ///     Appends the <c>SELECT DISTINCT</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="expression">The LambdaExpression.</param>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <returns>The <see cref="ISelectBuilder{TEntity}" /> instance.</returns>
    ISelectDistinctBuilder<TEntity> SelectDistinct<TResult>(Expression<Func<TEntity, TResult>> expression);
}
