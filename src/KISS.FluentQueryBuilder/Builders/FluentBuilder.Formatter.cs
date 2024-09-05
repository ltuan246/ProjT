namespace KISS.FluentQueryBuilder.Builders;

/// <summary>
///     Declares operations for the <see cref="FluentBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public sealed partial record FluentBuilder<TEntity>
{
    private void Append(string value)
        => StringBuilder.Append(value);

    private void AppendFormat(FormattableString formatString)
        => StringBuilder.AppendFormat(SqlFormatter, formatString.Format, formatString.GetArguments());
}
