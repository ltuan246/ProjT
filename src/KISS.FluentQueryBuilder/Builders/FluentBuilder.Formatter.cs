namespace KISS.FluentQueryBuilder.Builders;

public sealed partial class FluentBuilder
{
    private void Append(string value)
        => StringBuilder.Append(value);

    private void AppendFormat(FormattableString formatString)
        => StringBuilder.AppendFormat(Formatter, formatString.Format, formatString.GetArguments());
}
