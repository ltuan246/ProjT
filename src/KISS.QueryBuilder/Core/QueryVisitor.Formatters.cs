namespace KISS.QueryBuilder.Core;

/// <summary>
///     Implements the Formatter for the <see cref="QueryVisitor" /> type.
/// </summary>
internal sealed partial class QueryVisitor
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
        SqlBuilder.Append($"{type.Name}s {GetTableAlias(type)}");
        SqlBuilder.Append("  ");
    }
}
