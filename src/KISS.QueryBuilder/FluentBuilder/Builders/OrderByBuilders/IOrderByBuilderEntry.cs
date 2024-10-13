namespace KISS.QueryBuilder.FluentBuilder.Builders.OrderByBuilders;

/// <summary>
///     An interface that defines the order by entry builder type.
/// </summary>
public interface IOrderByBuilderEntry : IFluentSqlBuilder
{
    /// <summary>
    ///     Appends the <c>ORDER BY</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <returns>The <see cref="IOrderByBuilder" /> instance.</returns>
    IOrderByBuilder OrderBy();

    /// <summary>
    ///     Appends the <c>ORDER BY</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IOrderByBuilder" /> instance.</returns>
    IOrderByBuilder OrderBy(bool condition);
}
