namespace KISS.FluentSqlBuilder.Decorators;

/// <summary>
///     Provides core SQL query building utilities for the QueryDecorator.
///     This abstract base supplies methods for managing table aliases and appending SQL fragments,
///     supporting the construction of composite SQL queries with consistent formatting and structure.
/// </summary>
public abstract partial record QueryDecorator
{
    /// <summary>
    ///     Retrieves or creates a table alias for the specified type in the query context.
    ///     Ensures that each type used in the query has a unique and consistent alias,
    ///     which is reused throughout the query construction.
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
    ///     Use this method to add SQL fragments to the query without line breaks.
    /// </summary>
    /// <param name="value">The SQL string to append to the query.</param>
    public void Append(string value)
        => SqlBuilder.Append(value);

    /// <summary>
    ///     Appends a new line to the SQL query being built, with optional content.
    ///     Use this method to add SQL fragments with proper formatting and structure.
    /// </summary>
    /// <param name="value">The SQL string to append to the query after the line break.</param>
    public void AppendLine(string value = "")
    {
        SqlBuilder.AppendLine();
        SqlBuilder.Append(value);
    }
}
