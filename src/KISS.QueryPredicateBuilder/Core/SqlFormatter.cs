namespace KISS.QueryPredicateBuilder.Core;

internal sealed class SqlFormatter : IFormatProvider, ICustomFormatter
{
    private const string DefaultDatabaseParameterNameTemplate = "p";
    private const string DefaultDatabaseParameterPrefix = "@";

    public DynamicParameters Parameters { get; private set; } = new();

    private int ParamCount { get; set; }

    private string GetNextParameterName()
        => $"{DefaultDatabaseParameterNameTemplate}{ParamCount++}";

    private string AppendParameterPrefix(string parameterName)
        => $"{DefaultDatabaseParameterPrefix}{parameterName}";

    private string AddValueToParameters<T>(T value)
    {
        string parameterName = GetNextParameterName();
        Parameters.Add(parameterName, value, direction: ParameterDirection.Input);
        return AppendParameterPrefix(parameterName);
    }

    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
        => Format(arg, format);

    private string Format<T>(T value, string? format = null)
    {
        if (value is FormattableString formatString)
        {
            return formatString.ArgumentCount switch
            {
                0 => formatString.Format,
                _ => string.Format(this, formatString.Format, formatString.GetArguments())
            };
        }

        if (Constants.RawFormat.Equals(format, StringComparison.OrdinalIgnoreCase))
        {
            return value?.ToString() ?? string.Empty;
        }

        return AddValueToParameters(value);
    }

    public object? GetFormat(Type? formatType)
        => typeof(ICustomFormatter).IsAssignableFrom(formatType) switch
        {
            true => this,
            false => default
        };
}
