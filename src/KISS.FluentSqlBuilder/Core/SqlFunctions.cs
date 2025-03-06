namespace KISS.FluentSqlBuilder.Core;

/// <summary>
///     An expression that represents a function in a SQL.
/// </summary>
[ExcludeFromCodeCoverage]
public static class SqlFunctions
{
    /// <summary>
    ///     Retrieves data from a database based on conditions.
    /// </summary>
    /// <param name="dbConnection">The database connections.</param>
    /// <typeparam name="TEntity">The type of the record.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    public static QueryBuilder<TEntity> Retrieve<TEntity>(
        this DbConnection dbConnection)
        => new(dbConnection);

    /// <summary>
    ///     Appends the <c>BETWEEN</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="beginValue">The begin values.</param>
    /// <param name="endValue">The end values.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>Return the records where expression is within the range.</returns>
    public static bool InRange<TField>(TField field, TField beginValue, TField endValue)
    {
        _ = field;
        _ = beginValue;
        _ = endValue;
        return true;
    }

    /// <summary>
    ///     Appends an <c>IN</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="values">The values.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>An in filter.</returns>
    public static bool AnyIn<TField>(TField field, params TField[] values)
    {
        _ = field;
        _ = values;
        return true;
    }

    /// <summary>
    ///     Appends the <c>NOT IN</c> clause and the interpolated string to the builder.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="values">The values.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>An in filter.</returns>
    public static bool NotIn<TField>(TField field, params TField[] values)
    {
        _ = field;
        _ = values;
        return true;
    }
}
