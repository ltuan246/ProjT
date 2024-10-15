namespace KISS.QueryBuilder.FluentBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface that defines the select builder entry type.
/// </summary>
/// <typeparam name="TRecordset">The type in the recordset.</typeparam>
public interface ISelectBuilderEntry<TRecordset> : IFluentSqlBuilder
{
    /// <summary>
    ///     Appends the <c>SELECT</c> clause to the builder.
    /// </summary>
    /// <param name="columns">The table columns.</param>
    /// <returns>The <see cref="ISelectBuilder" /> instance.</returns>
    ISelectBuilder Select(Expression<Func<TRecordset, object>> columns);

    /// <summary>
    ///     Appends the <c>SELECT DISTINCT</c> clause to the builder.
    /// </summary>
    /// <param name="columns">The table columns.</param>
    /// <returns>The <see cref="ISelectBuilder" /> instance.</returns>
    ISelectDistinctBuilder SelectDistinct(Expression<Func<TRecordset, object>> columns);
}
