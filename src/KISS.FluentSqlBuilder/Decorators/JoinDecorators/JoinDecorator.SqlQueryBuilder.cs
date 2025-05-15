namespace KISS.FluentSqlBuilder.Decorators.JoinDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TIn">The type representing the database record set.</typeparam>
/// <typeparam name="TOut">The combined type to return.</typeparam>
public sealed partial record JoinDecorator<TIn, TOut>
{
    /// <summary>
    ///     Appends a formatted string to the SQL query being built.
    ///     This method is used for adding SQL fragments to the query without line breaks.
    /// </summary>
    /// <param name="value">The SQL string to append to the query.</param>
    private void Append(string value)
        => SqlBuilder.Append(value);

    /// <summary>
    ///     Appends a new line to the SQL query being built, with optional indentation.
    ///     This method is used for adding SQL fragments with proper formatting and structure.
    /// </summary>
    /// <param name="value">The SQL string to append to the query.</param>
    private void AppendLine(string value = "")
    {
        SqlBuilder.AppendLine();
        SqlBuilder.Append(value);
    }

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
    ///     Builds the complete SQL query by assembling all query clauses in the correct order.
    ///     This method orchestrates the construction of the final SQL query string.
    /// </summary>
    public void SetQueries()
    {
        SetSelect();
        SetFrom();
        SetJoin();
        SetWhere();
        SetOrderBy();
        SetLimit();
        SetOffset();
    }

    /// <summary>
    ///     Builds the SELECT clause of the SQL query by assembling column selections
    ///     and expressions from the stored statements.
    /// </summary>
    private void SetSelect()
        => new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Select])
            .AccessFirst(fs =>
            {
                Append("SELECT");
                AppendLine($"{fs}");
            })
            .AccessRemaining(fs =>
            {
                AppendLine($", {fs}");
            })
            .AccessLast(() => AppendLine())
            .Execute();

    /// <summary>
    ///     Builds the FROM clause of the SQL query by specifying the main table
    ///     and its alias from the table aliases collection.
    /// </summary>
    private void SetFrom()
    {
        var (table, alias) = TableAliases.First();
        Append("FROM");
        AppendLine($"{TypeUtils.GetTableName(table)} AS {alias}");
        AppendLine();
    }

    /// <summary>
    ///     Builds the JOIN clauses of the SQL query by assembling join conditions
    ///     from the stored statements.
    /// </summary>
    private void SetJoin()
        => new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Join])
            .AccessFirst(fs =>
            {
                AppendLine($"{fs}");
            })
            .AccessRemaining(fs =>
            {
                AppendLine($"{fs}");
            })
            .AccessLast(() => AppendLine())
            .Execute();

    /// <summary>
    ///     Builds the WHERE clause of the SQL query by assembling conditions
    ///     from the stored statements.
    /// </summary>
    private void SetWhere()
        => new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Where])
            .AccessFirst(fs =>
            {
                Append("WHERE");
                AppendLine($"{fs}");
            })
            .AccessRemaining(fs =>
            {
                AppendLine($"AND {fs}");
            })
            .AccessLast(() => AppendLine())
            .Execute();

    /// <summary>
    ///     Builds the ORDER BY clause of the SQL query by assembling sorting
    ///     conditions from the stored statements.
    /// </summary>
    private void SetOrderBy()
        => new EnumeratorProcessor<string>(SqlStatements[SqlStatement.OrderBy])
            .AccessFirst(fs =>
            {
                Append("ORDER BY");
                AppendLine($"{fs}");
            })
            .AccessRemaining(fs =>
            {
                AppendLine($", {fs}");
            })
            .AccessLast(() => AppendLine())
            .Execute();

    /// <summary>
    ///     Builds the LIMIT clause of the SQL query by adding the limit
    ///     value from the stored statements.
    /// </summary>
    private void SetLimit()
        => new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Limit])
            .AccessFirst(fs =>
            {
                Append("LIMIT");
                AppendLine($"{fs}");
                AppendLine();
            })
            .Execute();

    /// <summary>
    ///     Builds the OFFSET clause of the SQL query by adding the offset
    ///     value from the stored statements.
    /// </summary>
    private void SetOffset()
        => new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Offset])
            .AccessFirst(fs =>
            {
                Append("OFFSET");
                AppendLine($"{fs}");
                AppendLine();
            })
            .Execute();
}