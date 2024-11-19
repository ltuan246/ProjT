namespace KISS.QueryBuilder.Visitors;

/// <summary>
///     The core <see cref="QueryComponent" /> partial class.
/// </summary>
internal abstract partial class QueryComponent : IQueryComponent
{
    /// <summary>
    ///     Use to custom string formatting for SQL queries.
    /// </summary>
    protected virtual SqlFormatter SqlFormat { get; } = new();

    /// <summary>
    ///     A collection specifically for table aliases.
    /// </summary>
    protected virtual Dictionary<Type, string> TableAliases { get; } = [];

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    private bool HasOpenParentheses { get; set; }

    /// <inheritdoc />
    public abstract void Accept(IVisitor visitor);

    /// <summary>
    ///     Use to Open Parenthesis.
    /// </summary>
    protected void OpenParentheses()
    {
        HasOpenParentheses = true;
        SqlBuilder.Append(ClauseConstants.OpenParenthesis);
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
        SqlBuilder.Append(ClauseConstants.CloseParenthesis);
    }

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
