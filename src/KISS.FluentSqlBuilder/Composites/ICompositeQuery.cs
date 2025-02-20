namespace KISS.FluentSqlBuilder.Composites;

/// <summary>
///     Defines the contract for data retrieval operations.
///     This interface is responsible for retrieving various sets of data
///     through methods that apply common query setup logic and return
///     the results in different formats.
/// </summary>
public interface ICompositeQuery
{
    /// <summary>
    ///     The type representing the database record set.
    /// </summary>
    Type SourceEntity { get; }

    /// <summary>
    ///     The combined type to return.
    /// </summary>
    Type RetrieveEntity { get; }

    /// <summary>
    ///     Stores the mapping of properties.
    /// </summary>
    Dictionary<Type, (string, PropertyInfo[])> MapProfiles { get; }

    /// <summary>
    ///     Stores the group key.
    /// </summary>
    List<(string Alias, string Key, string GroupKey)> GroupKeys { get; }

    /// <summary>
    ///     A collection specifically for column aliases.
    /// </summary>
    List<string> ColumnAliases { get; }

    /// <summary>
    ///     Stores the group key.
    /// </summary>
    List<MemberAssignment> RetrievePropertyAssignmentProcessing { get; }

    /// <summary>
    ///     Stores the group key.
    /// </summary>
    List<(Type GroupingKeyType, Expression PropertyAssignment)> GroupingPropertyAssignmentProcessing { get; }

    /// <summary>
    ///     Stores the group key.
    /// </summary>
    ParameterExpression DapperRowVariable { get; }

    /// <summary>
    ///     Creates an expression that converts a given property expression to a specified target type
    ///     using Convert.ChangeType(). This ensures dynamic type conversion at runtime within an expression tree.
    /// </summary>
    /// <param name="targetProperty">The expression representing the property to convert.</param>
    /// <param name="targetType">The target type to which the property should be converted.</param>
    /// <returns>A UnaryExpression that represents the type conversion.</returns>
    UnaryExpression ChangeType(Expression targetProperty, Type targetType);

    /// <summary>
    ///     Appends a formatted string to the <see cref="SqlBuilder" />.
    /// </summary>
    /// <param name="value">The string to append.</param>
    void Append(string value);

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
    void AppendFormat(FormattableString formatString);

    /// <summary>
    ///     Appends a new line to the string being built.
    /// </summary>
    /// <param name="indent">Refers to adding spaces at the beginning of lines of text.</param>
    void AppendLine(bool indent = false);

    /// <summary>
    ///     Use to Open Parenthesis.
    /// </summary>
    void OpenParentheses();

    /// <summary>
    ///     Use to Open Parenthesis.
    /// </summary>
    void CloseParentheses();

    /// <summary>
    ///     Retrieves the alias mapped to the specified <see cref="Type" /> in the query context,
    ///     or creates a new alias if none exists.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> for which to retrieve or generate a table alias.</param>
    /// <returns>
    ///     A <see cref="string" /> representing the alias associated with the specified <paramref name="type" />.
    /// </returns>
    string GetAliasMapping(Type type);

    /// <summary>
    ///     Determines if a specific expression can be evaluated.
    /// </summary>
    /// <param name="node">The expression to visit.</param>
    /// <returns>
    ///     A tuple of a boolean (Evaluated) indicating whether the expression was evaluable,
    ///     and the evaluated result (Value) as a FormatString.
    /// </returns>
    (bool Evaluated, FormattableString Value) GetValue(Expression node);
}