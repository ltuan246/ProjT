namespace KISS.QueryBuilder.Core;

/// <summary>
///     Implements the Formatter for the <see cref="FluentSqlBuilder{TRecordset}" /> type.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public sealed partial class FluentSqlBuilder<TRecordset>
{
    private void Append(string value)
    {
        SqlBuilder.Append(value);
        SqlBuilder.Append("  ");
    }

    private void AppendFormat(FormattableString formatString)
        => SqlBuilder.AppendFormat(SqlFormat, formatString.Format, formatString.GetArguments());

    private void AppendTableAlias(Type type)
    {
        SqlBuilder.Append($"{type.Name} {GetTableAlias(type)}");
        SqlBuilder.Append("  ");
    }
}
