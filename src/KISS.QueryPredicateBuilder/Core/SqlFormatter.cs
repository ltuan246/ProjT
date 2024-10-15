namespace KISS.QueryPredicateBuilder.Core;

/// <summary>
///     Implements IFormatProvider and ICustomFormatter, which returns string information for supplied objects based on
///     custom criteria.
/// </summary>
internal sealed class SqlFormatter : IFormatProvider, ICustomFormatter
{
    private const string DefaultDatabaseParameterNameTemplate = "p";
    private const string DefaultDatabaseParameterPrefix = "@";

    /// <summary>
    ///     A dynamic object that can be passed to the Query method instead of normal parameters.
    /// </summary>
    public DynamicParameters Parameters { get; } = new();

    private int ParamCount { get; set; }

    /// <inheritdoc />
    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
        => Format(arg, format);

    /// <inheritdoc />
    public object? GetFormat(Type? formatType) => this;

    private string GetNextParameterName()
        => $"{DefaultDatabaseParameterNameTemplate}{ParamCount++}";

    private string AppendParameterPrefix(string parameterName)
        => $"{DefaultDatabaseParameterPrefix}{parameterName}";

    private string AddValueToParameters<T>(T value)
    {
        var parameterName = GetNextParameterName();
        Parameters.Add(parameterName, value, direction: ParameterDirection.Input);
        return AppendParameterPrefix(parameterName);
    }

    private string Format<T>(T value, string? format = null)
    {
        // if (value is FormattableString formatString)
        // {
        //     return formatString.ArgumentCount switch
        //     {
        //         0 => formatString.Format,
        //         _ => string.Format(this, formatString.Format, formatString.GetArguments())
        //     };
        // }

        if (Constants.RawFormat.Equals(format, StringComparison.OrdinalIgnoreCase))
        {
            return value?.ToString() ?? string.Empty;
        }

        return AddValueToParameters(value);
    }

    // => typeof(ICustomFormatter).IsAssignableFrom(formatType) switch
    // {
    //     true => this,
    //     false => default
    // };
}
