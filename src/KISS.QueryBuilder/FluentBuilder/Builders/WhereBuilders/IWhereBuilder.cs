namespace KISS.QueryBuilder.FluentBuilder.Builders.WhereBuilders;

/// <summary>
///     An interface that defines the where builder type.
/// </summary>
internal interface IWhereBuilder : IGroupByBuilder
{
    /// <summary>
    ///     Appends the <c>WHERE</c> clause and the interpolated string to the builder.
    ///     If the <c>WHERE</c> clause is already present, the <c>AND</c> clause is appended.
    /// </summary>
    /// <returns>The <see cref="IWhereBuilder" /> instance.</returns>
    IWhereBuilder Where();

    /// <summary>
    ///     Appends the <c>WHERE</c> clause and the interpolated string to the builder.
    ///     If the <c>WHERE</c> clause is already present, the <c>AND</c> clause is appended.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IWhereBuilder" /> instance.</returns>
    IWhereBuilder Where(bool condition);
}
