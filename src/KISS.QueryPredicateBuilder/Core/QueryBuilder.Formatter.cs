namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder
{
    private void AppendFormattable(FormattableString formattable)
        => _ = formattable.ArgumentCount switch
        {
            0 => StateBuilder.Append(formattable.Format),
            _ => StateBuilder.AppendFormat(Formatter, formattable.Format, formattable.GetArguments())
        };
}