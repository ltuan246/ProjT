namespace KISS.FluentQueryBuilder.Core;

/// <summary>
/// Implements IFormatProvider and ICustomFormatter, which returns string information for supplied objects based on custom criteria.
/// </summary>
internal sealed class SqlFormatter : IFormatProvider, ICustomFormatter
{
    private const string DefaultDatabaseParameterNameTemplate = "p";
    private const string DefaultDatabaseParameterPrefix = "@";

    /// <summary>
    /// A dynamic object that can be passed to the Query method instead of normal parameters.
    /// </summary>
    public DynamicParameters Parameters { get; } = new();

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

    /// <inheritdoc />
    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
        => AddValueToParameters(arg);

    /// <inheritdoc />
    public object GetFormat(Type? formatType) => this;
}
