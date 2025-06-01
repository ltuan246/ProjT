namespace KISS.FluentSqlBuilder.Core;

/// <summary>
///     Provides a set of static methods for building SQL queries in a fluent manner.
///     This class serves as the entry point for constructing SQL queries using
///     strongly-typed expressions and LINQ-like syntax.
/// </summary>
[ExcludeFromCodeCoverage]
public static class SqlFunctions
{
    /// <summary>
    ///     Creates a new query builder for retrieving data from the database.
    ///     This method serves as the entry point for building SQL queries.
    /// </summary>
    /// <param name="dbConnection">The database connection to use for the query.</param>
    /// <typeparam name="TEntity">The type of entity to query, representing the database table.</typeparam>
    /// <returns>
    ///     A new <see cref="QueryBuilder{TEntity}" /> instance that can be used to
    ///     build and execute the SQL query.
    /// </returns>
    public static QueryBuilder<TEntity> Retrieve<TEntity>(
        this DbConnection dbConnection)
        => new(dbConnection);

    /// <summary>
    ///     Creates a BETWEEN condition for a field, checking if its value falls within
    ///     a specified range (inclusive).
    /// </summary>
    /// <param name="field">The field to check.</param>
    /// <param name="beginValue">The lower bound of the range (inclusive).</param>
    /// <param name="endValue">The upper bound of the range (inclusive).</param>
    /// <typeparam name="TField">The type of the field and range values.</typeparam>
    /// <returns>Always returns true; the actual filtering is handled by the query builder.</returns>
    public static bool InRange<TField>(TField field, TField beginValue, TField endValue)
    {
        _ = field;
        _ = beginValue;
        _ = endValue;
        return true;
    }

    /// <summary>
    ///     Creates an IN condition for a field, checking if its value matches any
    ///     of the specified values.
    /// </summary>
    /// <param name="field">The field to check.</param>
    /// <param name="values">The values to check against.</param>
    /// <typeparam name="TField">The type of the field and values.</typeparam>
    /// <returns>Always returns true; the actual filtering is handled by the query builder.</returns>
    public static bool AnyIn<TField>(TField field, params TField[] values)
    {
        _ = field;
        _ = values;
        return true;
    }

    /// <summary>
    ///     Creates a NOT IN condition for a field, checking if its value does not match
    ///     any of the specified values.
    /// </summary>
    /// <param name="field">The field to check.</param>
    /// <param name="values">The values to check against.</param>
    /// <typeparam name="TField">The type of the field and values.</typeparam>
    /// <returns>Always returns true; the actual filtering is handled by the query builder.</returns>
    public static bool NotIn<TField>(TField field, params TField[] values)
    {
        _ = field;
        _ = values;
        return true;
    }
}
