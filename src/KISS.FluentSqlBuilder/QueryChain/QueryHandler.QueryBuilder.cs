namespace KISS.FluentSqlBuilder.QueryChain;

/// <summary>
///     Provides functionality for building SQL query statements in a fluent manner.
///     This partial class extends the QueryHandler with methods for constructing
///     SQL statements using a StringBuilder and managing query formatting.
/// </summary>
public abstract partial record QueryHandler
{
    private string Sql
        => SqlBuilder.ToString();

    /// <summary>
    ///     Gets the StringBuilder used to construct the SQL statement.
    ///     This property provides access to the underlying string builder
    ///     for accumulating SQL query components.
    /// </summary>
    protected StringBuilder SqlBuilder { get; } = new();

    /// <summary>
    ///     Tracks whether there is an open parenthesis in the current SQL statement.
    ///     Used to ensure proper nesting and closure of parentheses in complex expressions.
    /// </summary>
    private bool HasOpenParentheses { get; set; }

    /// <summary>
    ///     Appends a string value to the SQL statement being built.
    ///     This method is used for adding static SQL components that don't require
    ///     formatting or parameter substitution.
    /// </summary>
    /// <param name="value">The string to append to the SQL statement.</param>
    protected void Append(string value)
        => SqlBuilder.Append(value);

    /// <summary>
    ///     Appends a string value to the SQL statement with optional indentation.
    ///     This method is used for adding SQL components that should be on a new line,
    ///     with proper formatting and indentation for readability.
    /// </summary>
    /// <param name="value">The string to append to the SQL statement.</param>
    /// <param name="indent">Whether to indent the appended line.</param>
    protected void AppendLine(string value = "", bool indent = false)
    {
        SqlBuilder.AppendLine();
        if (indent)
        {
            const int indentationLevel = 4;
            SqlBuilder.Append(new string(' ', indentationLevel));
        }

        SqlBuilder.Append(value);
    }

    /// <summary>
    ///     Appends a formatted string to the SQL statement using the SQL formatter.
    ///     This method handles parameter substitution and proper SQL formatting
    ///     for dynamic query components.
    /// </summary>
    /// <param name="formatString">
    ///     A FormattableString containing the SQL statement with placeholders.
    /// </param>
    protected void AppendFormat(FormattableString formatString)
        => SqlBuilder.AppendFormat(Composite.SqlFormatting, formatString.Format, formatString.GetArguments());

    /// <summary>
    ///     Opens a parenthesis in the SQL statement.
    ///     This method is used to start a parenthesized expression and tracks
    ///     the parenthesis state for proper nesting.
    /// </summary>
    protected void OpenParentheses()
    {
        HasOpenParentheses = true;
        const char openParenthesis = '(';
        SqlBuilder.Append(openParenthesis);
    }

    /// <summary>
    ///     Closes a parenthesis in the SQL statement if one is open.
    ///     This method ensures proper closure of parenthesized expressions
    ///     and maintains correct SQL syntax.
    /// </summary>
    protected void CloseParentheses()
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
