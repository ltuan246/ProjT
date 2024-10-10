namespace KISS.QueryBuilder.FluentBuilder.Builders.HavingBuilders;

/// <summary>
///     An interface that defines the having builder type.
/// </summary>
internal interface IHavingBuilder : IOrderByBuilderEntry
{
    /// <summary>
    ///     Appends the <c>HAVING</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <returns>The <see cref="IHavingBuilder" /> instance.</returns>
    IHavingBuilder Having();

    /// <summary>
    ///     Appends the <c>HAVING</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IHavingBuilder" /> instance.</returns>
    IHavingBuilder Having(bool condition);
}
