namespace KISS.QueryBuilder.Core;

/// <summary>
///     Implements the Formatter for the <see cref="FluentSqlBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of results to return.</typeparam>
public sealed partial class FluentSqlBuilder<TEntity>
{
    private void Append(string value)
        => SqlBuilder.Append(value);

    private void AppendFormat(FormattableString formatString)
        => SqlBuilder.AppendFormat(SqlFormat, formatString.Format, formatString.GetArguments());
}
