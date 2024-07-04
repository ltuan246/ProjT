namespace KISS.QueryPredicateBuilder.Core;

internal sealed class SqlFormatter : IFormatProvider, ICustomFormatter
{
    internal const string DefaultDatabaseParameterNameTemplate = "p";
    internal const string DefaultDatabaseParameterPrefix = "@";

    public DynamicParameters Parameters { get; private set; } = new();

    private int ParamCount { get; set; }

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

    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
        => Format(arg);

    public string Format<T>(T value)
    {
        if (value is FormattableString formattableString)
        {
            return formattableString.ArgumentCount switch
            {
                0 => formattableString.Format,
                _ => string.Format(this, formattableString.Format, formattableString.GetArguments())
            };
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
