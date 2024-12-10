namespace KISS.FluentSqlBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface for adding a <c>FROM</c> clause to the query.
/// </summary>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISelectFromBuilderEntry<TReturn> : ISqlBuilder
{
    /// <summary>
    ///     An interface for adding a <c>FROM</c> clause to the query.
    /// </summary>
    /// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TRecordset, TReturn}"/> instance.</returns>
    IJoinBuilder<TRecordset, TReturn> From<TRecordset>();
}
