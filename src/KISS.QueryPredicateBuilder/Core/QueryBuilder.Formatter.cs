namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder
{
    private void AppendFormatString(FormattableString formatString)
        => _ = formatString.ArgumentCount switch
        {
            0 => StateBuilder.Append(formatString.Format),
            _ => StateBuilder.AppendFormat(Formatter, formatString.Format, formatString.GetArguments())
        };
}