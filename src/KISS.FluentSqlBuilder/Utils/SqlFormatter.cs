namespace KISS.FluentSqlBuilder.Utils;

/// <summary>
///     Implements <see cref="IFormatProvider" /> and <see cref="ICustomFormatter" />
///     to provide custom string formatting for SQL queries.
/// </summary>
public sealed record SqlFormatter : IFormatProvider, ICustomFormatter
{
    /// <summary>
    ///     A collection of dynamic parameters that can be used in SQL queries.
    /// </summary>
    public DynamicParameters Parameters { get; } = new();

    /// <summary>
    ///     Keeps track of the number of parameters added.
    /// </summary>
    private int ParamCount { get; set; }

    /// <inheritdoc />
    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        const string defaultParameterName = "p";
        const string defaultParameterPrefix = "@";

        var parameterName = $"{defaultParameterName}{ParamCount++}";
        Parameters.Add(parameterName, arg, direction: ParameterDirection.Input);
        return $"{defaultParameterPrefix}{parameterName}";
    }

    /// <inheritdoc />
    public object GetFormat(Type? formatType) => this;
}
