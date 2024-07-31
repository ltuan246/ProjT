namespace KISS.QueryPredicateBuilder.Core;

/// <summary>
/// Implements Custom Formatter, which returns string information for supplied objects based on custom criteria.
/// </summary>
public sealed partial class QueryBuilder
{
    private void AppendFormatString(FormattableString formatString)
        => _ = formatString.ArgumentCount switch
        {
            0 => StateBuilder.Append(formatString.Format),
            _ => StateBuilder.AppendFormat(Formatter, formatString.Format, formatString.GetArguments())
        };
}
