namespace KISS.FluentSqlBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface for adding a <c>FROM</c> clause to the query.
///     Provides the entry point for specifying the source table or view for the query.
/// </summary>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISelectFromBuilderEntry<TReturn> : ISqlBuilder
{
    /// <summary>
    ///     Adds a <c>FROM</c> clause to the query, specifying the source table.
    /// </summary>
    /// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TRecordset, TReturn}" /> instance for method chaining.</returns>
    IJoinBuilder<TRecordset, TReturn> From<TRecordset>();
}
