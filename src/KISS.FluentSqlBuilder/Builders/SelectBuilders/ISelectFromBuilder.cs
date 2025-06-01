namespace KISS.FluentSqlBuilder.Builders.SelectBuilders;

/// <summary>
///     Defines an interface for building the FROM clause of a SQL query. This interface
///     provides the foundation for specifying the source table or view in a SELECT statement.
/// </summary>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface ISelectFromBuilder<TReturn>
{
    /// <summary>
    ///     Adds a FROM clause to the query, specifying the source table or view.
    ///     This method initiates the query building process by defining the primary
    ///     data source for the SELECT statement.
    /// </summary>
    /// <typeparam name="TRecordset">The type representing the database table or view to query.</typeparam>
    /// <returns>
    ///     An <see cref="IJoinBuilder{TRecordset, TReturn}" /> instance that allows you to
    ///     add joins and other query components.
    /// </returns>
    IJoinBuilder<TRecordset, TReturn> From<TRecordset>();
}
