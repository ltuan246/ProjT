namespace KISS.FluentQueryBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface that defines the select distinct builder entry type.
/// </summary>
public interface ISelectDistinctBuilderEntry
{
    /// <summary>
    ///     Appends the <c>SELECT DISTINCT</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="expression">The LambdaExpression.</param>
    /// <returns>The <see cref="ISelectBuilder{TEntity}" /> instance.</returns>
    ISelectDistinctBuilder SelectDistinct(LambdaExpression expression);
}
