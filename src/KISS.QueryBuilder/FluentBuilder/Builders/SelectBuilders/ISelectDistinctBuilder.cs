namespace KISS.QueryBuilder.FluentBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface that defines the select distinct builder type.
/// </summary>
internal interface ISelectDistinctBuilder : IFluentSqlBuilder
{
    /// <summary>
    ///     Appends the <c>SELECT DISTINCT</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <returns>The <see cref="ISelectDistinctBuilder" /> instance.</returns>
    ISelectDistinctBuilder SelectDistinct();
}
