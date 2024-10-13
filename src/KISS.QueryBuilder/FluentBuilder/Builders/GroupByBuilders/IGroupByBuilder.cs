namespace KISS.QueryBuilder.FluentBuilder.Builders.GroupByBuilders;

/// <summary>
///     An interface that defines the group by builder type.
/// </summary>
public interface IGroupByBuilder : IHavingBuilder
{
    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <returns>The <see cref="IGroupByBuilder" /> instance.</returns>
    IGroupByBuilder GroupBy();

    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IGroupByBuilder" /> instance.</returns>
    IGroupByBuilder GroupBy(bool condition);
}
