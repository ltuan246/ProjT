namespace KISS.FluentSqlBuilder.Decorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public abstract partial record QueryDecorator
{
    /// <summary>
    ///     Retrieves or creates a table alias for the specified type in the query context.
    ///     This method ensures consistent alias usage throughout the query construction.
    /// </summary>
    /// <param name="type">The type for which to retrieve or generate a table alias.</param>
    /// <returns>
    ///     A string representing the alias associated with the specified type.
    ///     If no alias exists, a new one is generated and stored.
    /// </returns>
    public string GetAliasMapping(Type type)
    {
        if (!TableAliases.TryGetValue(type, out var tableAlias))
        {
            const string defaultTableAlias = "Extend";
            tableAlias = $"{defaultTableAlias}{TableAliases.Count}";
            TableAliases.Add(type, tableAlias);
        }

        return tableAlias;
    }

    /// <summary>
    ///     Appends a formatted string to the SQL query being built.
    ///     This method is used for adding SQL fragments to the query without line breaks.
    /// </summary>
    /// <param name="value">The SQL string to append to the query.</param>
    public void Append(string value)
        => SqlBuilder.Append(value);

    /// <summary>
    ///     Appends a new line to the SQL query being built, with optional indentation.
    ///     This method is used for adding SQL fragments with proper formatting and structure.
    /// </summary>
    /// <param name="value">The SQL string to append to the query.</param>
    public void AppendLine(string value = "")
    {
        SqlBuilder.AppendLine();
        SqlBuilder.Append(value);
    }
}
