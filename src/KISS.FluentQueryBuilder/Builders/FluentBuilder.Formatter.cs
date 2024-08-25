namespace KISS.FluentQueryBuilder.Builders;

public sealed partial record FluentBuilder<TEntity>
{
    private void Append(string value)
        => StringBuilder.Append(value);

    private void AppendFormat(FormattableString formatString)
        => StringBuilder.AppendFormat(SqlFormatter, formatString.Format, formatString.GetArguments());
}
