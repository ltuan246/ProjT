namespace KISS.QueryBuilder.FluentBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface that defines the select builder type.
/// </summary>
internal interface ISelectBuilder : IFluentSqlBuilder
{
    /// <summary>
    ///     Appends the <c>SELECT</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <returns>The <see cref="ISelectBuilder" /> instance.</returns>
    ISelectBuilder Select();
}
