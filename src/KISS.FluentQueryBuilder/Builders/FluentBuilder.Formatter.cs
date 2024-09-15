namespace KISS.FluentQueryBuilder.Builders;

/// <summary>
///     Declares operations for the <see cref="FluentBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public sealed partial class FluentBuilder<TEntity>
{
    private void Append(string value)
        => StringBuilder.Append(value);

    private void AppendFormat(FormattableString formatString)
        => StringBuilder.AppendFormat(Formatter, formatString.Format, formatString.GetArguments());
}
