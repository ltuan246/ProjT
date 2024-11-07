namespace KISS.QueryBuilder.Visitors;

/// <summary>
///     The core <see cref="QueryComponent" /> partial class.
/// </summary>
internal abstract partial record QueryComponent
{
    /// <summary>
    ///     Use to custom string formatting for SQL queries.
    /// </summary>
    public abstract SqlFormatter SqlFormat { get; init; }

    /// <summary>
    ///     A collection specifically for table aliases.
    /// </summary>
    public abstract Dictionary<Type, string> TableAliases { get; init; }

    /// <summary>
    ///     Retrieves the alias mapped to the specified <see cref="Type" /> in the query context,
    ///     or creates a new alias if none exists.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> for which to retrieve or generate a table alias.</param>
    /// <returns>
    ///     A <see cref="string" /> representing the alias associated with the specified <paramref name="type" />.
    /// </returns>
    protected string GetAliasMapping(Type type)
    {
        if (!TableAliases.TryGetValue(type, out var tableAlias))
        {
            // Generate a new alias based on the default alias prefix and current alias count.
            tableAlias = $"{ClauseConstants.DefaultTableAlias}{TableAliases.Count}";

            // Store the new alias in the dictionary for future reference.
            TableAliases.Add(type, tableAlias);
        }

        // Return the alias (existing or newly created) for the specified type.
        return tableAlias;
    }
}
