namespace KISS.FluentSqlBuilder.QueryHandlerChain;

/// <summary>
///     QueryHandler.
/// </summary>
public abstract partial record QueryHandler
{
    /// <summary>
    ///     Sets the generated the SQL.
    /// </summary>
    protected StringBuilder QueryBuilder { get; } = new();

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    private bool HasOpenParentheses { get; set; }

    /// <summary>
    ///     Appends a formatted string to the <see cref="QueryBuilder" />.
    /// </summary>
    /// <param name="value">The string to append.</param>
    protected void Append(string value)
        => QueryBuilder.Append(value);

    /// <summary>
    ///     Appends a formatted string to the <see cref="QueryBuilder" /> using the specified SQL format provider.
    /// </summary>
    /// <param name="formatString">
    ///     A <see cref="FormattableString" /> instance containing the composite format string and arguments.
    ///     The format string specifies the text, placeholders, and data for the formatted SQL statement.
    /// </param>
    /// <remarks>
    ///     This method formats and appends a SQL statement to the underlying <see cref="QueryBuilder" /> by using
    ///     the <see cref="SqlFormatter" /> as a format provider. The <see cref="FormattableString.Format" /> and
    ///     <see cref="FormattableString.GetArguments" /> methods are used to parse the format string and its arguments
    ///     before appending the formatted result to <see cref="QueryBuilder" />.
    ///     Example usage:
    ///     <code>
    ///     AppendFormat($"SELECT * FROM Orders WHERE OrderId = {orderId}");
    ///     </code>
    ///     This example will append a SQL statement with a placeholder for <c>OrderId</c>,
    ///     formatted by <see cref="SqlFormatter" />.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatString" /> is <c>null</c>.</exception>
    protected void AppendFormat(FormattableString formatString)
        => QueryBuilder.AppendFormat(Composite.SqlFormatting, formatString.Format, formatString.GetArguments());

    /// <summary>
    ///     Use to Open Parenthesis.
    /// </summary>
    protected void OpenParentheses()
    {
        HasOpenParentheses = true;
        const char openParenthesis = '(';
        QueryBuilder.Append(openParenthesis);
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
        QueryBuilder.Append(closeParenthesis);
    }
}
