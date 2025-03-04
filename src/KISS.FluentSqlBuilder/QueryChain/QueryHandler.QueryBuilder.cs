namespace KISS.FluentSqlBuilder.QueryChain;

/// <summary>
///     QueryHandler.
/// </summary>
public abstract partial record QueryHandler
{
    /// <summary>
    ///     Sets the generated the SQL.
    /// </summary>
    protected StringBuilder StatementBuilder { get; } = new();

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    private bool HasOpenParentheses { get; set; }

    /// <summary>
    ///     Appends a formatted string to the <see cref="StatementBuilder" />.
    /// </summary>
    /// <param name="value">The string to append.</param>
    protected void Append(string value)
        => StatementBuilder.Append(value);

    /// <summary>
    ///     Appends a new line to the string being built.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <param name="indent">Refers to adding spaces at the beginning of lines of text.</param>
    protected void AppendLine(string value = "", bool indent = false)
    {
        StatementBuilder.AppendLine();
        if (indent)
        {
            const int indentationLevel = 4;
            StatementBuilder.Append(new string(' ', indentationLevel));
        }

        StatementBuilder.Append(value);
    }

    /// <summary>
    ///     Appends a formatted string to the <see cref="StatementBuilder" /> using the specified SQL format provider.
    /// </summary>
    /// <param name="formatString">
    ///     A <see cref="FormattableString" /> instance containing the composite format string and arguments.
    ///     The format string specifies the text, placeholders, and data for the formatted SQL statement.
    /// </param>
    /// <remarks>
    ///     This method formats and appends a SQL statement to the underlying <see cref="StatementBuilder" /> by using
    ///     the <see cref="SqlFormatter" /> as a format provider. The <see cref="FormattableString.Format" /> and
    ///     <see cref="FormattableString.GetArguments" /> methods are used to parse the format string and its arguments
    ///     before appending the formatted result to <see cref="StatementBuilder" />.
    ///     Example usage:
    ///     <code>
    ///     AppendFormat($"SELECT * FROM Orders WHERE OrderId = {orderId}");
    ///     </code>
    ///     This example will append a SQL statement with a placeholder for <c>OrderId</c>,
    ///     formatted by <see cref="SqlFormatter" />.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is <c>null</c>.</exception>
    protected void AppendFormat(FormattableString formatString)
        => StatementBuilder.AppendFormat(Composite.SqlFormatting, formatString.Format, formatString.GetArguments());

    /// <summary>
    ///     Use to Open Parenthesis.
    /// </summary>
    protected void OpenParentheses()
    {
        HasOpenParentheses = true;
        const char openParenthesis = '(';
        StatementBuilder.Append(openParenthesis);
    }

    /// <summary>
    ///     Use to Close Parenthesis.
    /// </summary>
    protected void CloseParentheses()
    {
        if (!HasOpenParentheses)
        {
            return;
        }

        HasOpenParentheses = false;
        const char closeParenthesis = ')';
        StatementBuilder.Append(closeParenthesis);
    }
}
