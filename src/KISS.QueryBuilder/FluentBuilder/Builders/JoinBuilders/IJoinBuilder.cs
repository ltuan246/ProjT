namespace KISS.QueryBuilder.FluentBuilder.Builders.JoinBuilders;

/// <summary>
///     An interface that defines the join builder type.
/// </summary>
public interface IJoinBuilder : IWhereBuilder
{
    /// <summary>
    ///     Appends the <c>INNER JOIN</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <returns>The <see cref="IJoinBuilder" /> instance.</returns>
    IJoinBuilder InnerJoin();

    /// <summary>
    ///     Appends the <c>INNER JOIN</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IJoinBuilder" /> instance.</returns>
    IJoinBuilder InnerJoin(bool condition);

    /// <summary>
    ///     Appends the <c>LEFT JOIN</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <returns>The <see cref="IJoinBuilder" /> instance.</returns>
    IJoinBuilder LeftJoin();

    /// <summary>
    ///     Appends the <c>LEFT JOIN</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IJoinBuilder" /> instance.</returns>
    IJoinBuilder LeftJoin(bool condition);

    /// <summary>
    ///     Appends the <c>RIGHT JOIN</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <returns>The <see cref="IJoinBuilder" /> instance.</returns>
    IJoinBuilder RightJoin();

    /// <summary>
    ///     Appends the <c>RIGHT JOIN</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IJoinBuilder" /> instance.</returns>
    IJoinBuilder RightJoin(bool condition);
}
