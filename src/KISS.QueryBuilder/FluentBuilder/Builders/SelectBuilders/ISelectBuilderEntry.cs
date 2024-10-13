namespace KISS.QueryBuilder.FluentBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface that defines the select builder entry type.
/// </summary>
public interface ISelectBuilderEntry : IFluentSqlBuilder
{
    /// <summary>
    ///     Appends the <c>SELECT</c> clause to the builder.
    /// </summary>
    /// <param name="columns">The table columns.</param>
    /// <typeparam name="TRecordset">The type in the recordset.</typeparam>
    /// <typeparam name="TResult">The combined type to return.</typeparam>
    /// <returns>The <see cref="ISelectBuilder" /> instance.</returns>
    ISelectBuilder Select<TRecordset, TResult>(Expression<Func<TRecordset, TResult>> columns);

    /// <summary>
    ///     Appends the <c>SELECT DISTINCT</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <returns>The <see cref="ISelectDistinctBuilder" /> instance.</returns>
    ISelectDistinctBuilder SelectDistinct();
}
