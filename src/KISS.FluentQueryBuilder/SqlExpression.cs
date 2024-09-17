namespace KISS.FluentQueryBuilder;

/// <summary>
///     An expression that represents a function in a SQL.
/// </summary>
[ExcludeFromCodeCoverage]
public static class SqlExpression
{
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
