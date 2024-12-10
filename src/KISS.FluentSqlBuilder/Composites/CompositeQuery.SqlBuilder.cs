namespace KISS.FluentSqlBuilder.Composites;

/// <summary>
///     A class that defines the fluent SQL builder type.
/// </summary>
public sealed partial class CompositeQuery
{
    /// <summary>
    ///     Gets the generated the SQL.
    /// </summary>
    public string Sql
        => SqlBuilder.ToString();

    /// <summary>
    ///     A collection of dynamic parameters that can be used in SQL queries.
    /// </summary>
    public DynamicParameters Parameters
        => SqlFormat.Parameters;

    /// <summary>
    ///     Sets the generated the SQL.
    /// </summary>
    private StringBuilder SqlBuilder { get; } = new();

    /// <summary>
    ///     Use to custom string formatting for SQL queries.
    /// </summary>
    private SqlFormatter SqlFormat { get; } = new();

    /// <summary>
    ///     A collection specifically for table aliases.
    /// </summary>
    private Dictionary<Type, string> TableAliases { get; } = [];

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    private bool HasOpenParentheses { get; set; }

    /// <summary>
    ///     Appends a formatted string to the <see cref="SqlBuilder" />.
    /// </summary>
    /// <param name="value">The string to append.</param>
    public void Append(string value)
        => SqlBuilder.Append(value);

    /// <summary>
    ///     Appends a new line to the string being built.
    /// </summary>
    /// <param name="indent">Refers to adding spaces at the beginning of lines of text.</param>
    public void AppendLine(bool indent = false)
    {
        SqlBuilder.AppendLine();
        if (indent)
        {
            const int indentationLevel = 4;
            SqlBuilder.Append(new string(' ', indentationLevel));
        }
    }

    /// <summary>
    ///     Appends a formatted string to the <see cref="SqlBuilder" /> using the specified SQL format provider.
    /// </summary>
    /// <param name="formatString">
    ///     A <see cref="FormattableString" /> instance containing the composite format string and arguments.
    ///     The format string specifies the text, placeholders, and data for the formatted SQL statement.
    /// </param>
    /// <remarks>
    ///     This method formats and appends a SQL statement to the underlying <see cref="SqlBuilder" /> by using
    ///     the <see cref="SqlFormatter" /> as a format provider. The <see cref="FormattableString.Format" /> and
    ///     <see cref="FormattableString.GetArguments" /> methods are used to parse the format string and its arguments
    ///     before appending the formatted result to <see cref="SqlBuilder" />.
    ///     Example usage:
    ///     <code>
    ///     AppendFormat($"SELECT * FROM Orders WHERE OrderId = {orderId}");
    ///     </code>
    ///     This example will append a SQL statement with a placeholder for <c>OrderId</c>,
    ///     formatted by <see cref="SqlFormatter" />.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is <c>null</c>.</exception>
    public void AppendFormat(FormattableString formatString)
        => SqlBuilder.AppendFormat(SqlFormat, formatString.Format, formatString.GetArguments());

    /// <summary>
    ///     Retrieves the alias mapped to the specified <see cref="Type" /> in the query context,
    ///     or creates a new alias if none exists.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> for which to retrieve or generate a table alias.</param>
    /// <returns>
    ///     A <see cref="string" /> representing the alias associated with the specified <paramref name="type" />.
    /// </returns>
    public string GetAliasMapping(Type type)
    {
        if (!TableAliases.TryGetValue(type, out var tableAlias))
        {
            const string defaultTableAlias = "Extend";

            // Generate a new alias based on the default alias prefix and current alias count.
            tableAlias = $"{defaultTableAlias}{TableAliases.Count}";

            // Store the new alias in the dictionary for future reference.
            TableAliases.Add(type, tableAlias);
        }

        // Return the alias (existing or newly created) for the specified type.
        return tableAlias;
    }

    /// <summary>
    ///     Use to Open Parenthesis.
    /// </summary>
    public void OpenParentheses()
    {
        HasOpenParentheses = true;
        const char openParenthesis = '(';
        SqlBuilder.Append(openParenthesis);
    }

    /// <summary>
    ///     Use to Close Parenthesis.
    /// </summary>
    public void CloseParentheses()
    {
        if (!HasOpenParentheses)
        {
            return;
        }

        HasOpenParentheses = false;
        const char closeParenthesis = ')';
        SqlBuilder.Append(closeParenthesis);
    }
}
