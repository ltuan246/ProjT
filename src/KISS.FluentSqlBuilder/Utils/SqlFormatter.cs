namespace KISS.FluentSqlBuilder.Utils;

/// <summary>
///     Implements <see cref="IFormatProvider" /> and <see cref="ICustomFormatter" />
///     to provide custom string formatting for SQL queries. This class handles the
///     conversion of C# expressions into parameterized SQL queries for safe execution.
/// </summary>
public sealed record SqlFormatter : IFormatProvider, ICustomFormatter
{
    /// <summary>
    ///     A collection of dynamic parameters that can be used in SQL queries.
    ///     This property stores the parameters that will be used when executing
    ///     the SQL query, ensuring safe parameter substitution.
    /// </summary>
    public DynamicParameters Parameters { get; } = new();

    /// <summary>
    ///     Keeps track of the number of parameters added to the query.
    ///     This counter is used to generate unique parameter names in the
    ///     format @p0, @p1, etc.
    /// </summary>
    private int ParamCount { get; set; }

    /// <inheritdoc />
    /// <summary>
    ///     Formats the specified value into a SQL parameter placeholder.
    /// </summary>
    /// <param name="format">The format string (not used in this implementation).</param>
    /// <param name="arg">The value to be formatted into a SQL parameter.</param>
    /// <param name="formatProvider">The format provider (not used in this implementation).</param>
    /// <returns>
    ///     A string containing the SQL parameter placeholder in the format @pN,
    ///     where N is the parameter index.
    /// </returns>
    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        const string defaultParameterName = "p";
        const string defaultParameterPrefix = "@";

        var parameterName = $"{defaultParameterName}{ParamCount++}";
        Parameters.Add(parameterName, arg, direction: ParameterDirection.Input);
        return $"{defaultParameterPrefix}{parameterName}";
    }

    /// <inheritdoc />
    /// <summary>
    ///     Returns this instance as the format provider for SQL formatting.
    /// </summary>
    /// <param name="formatType">The type of format object to get (not used).</param>
    /// <returns>This instance as the format provider.</returns>
    public object GetFormat(Type? formatType) => this;
}
