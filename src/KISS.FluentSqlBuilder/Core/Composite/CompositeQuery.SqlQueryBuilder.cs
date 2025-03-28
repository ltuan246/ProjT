namespace KISS.FluentSqlBuilder.Core.Composite;

/// <summary>
///     A partial class that provides SQL query building functionality for the CompositeQuery class.
///     This class handles the construction of SQL queries by assembling various SQL clauses
///     and managing query formatting and structure.
/// </summary>
public sealed partial class CompositeQuery
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
    /// <param name="indent">Whether to add indentation to the new line.</param>
    private void AppendLine(string value = "", bool indent = false)
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
        SetGroupBy();
        SetOrderBy();
        SetLimit();
        SetOffset();
    }

    /// <summary>
    ///     Builds the SELECT clause of the SQL query by assembling column selections
    ///     and expressions from the stored statements.
    /// </summary>
    private void SetSelect()
        => new EnumeratorProcessor<FormattableString>(SqlStatements[SqlStatement.Select])
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
        AppendLine($"{table.Name} AS {alias}");
        AppendLine();
    }

    /// <summary>
    ///     Builds the JOIN clauses of the SQL query by assembling join conditions
    ///     from the stored statements.
    /// </summary>
    private void SetJoin()
        => new EnumeratorProcessor<FormattableString>(SqlStatements[SqlStatement.Join])
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
        => new EnumeratorProcessor<FormattableString>(SqlStatements[SqlStatement.Where])
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
    ///     Builds the GROUP BY clause of the SQL query, including handling of
    ///     Common Table Expressions (CTEs) for complex grouping scenarios.
    /// </summary>
    private void SetGroupBy()
        => new EnumeratorProcessor<FormattableString>(SqlStatements[SqlStatement.GroupBy])
            .AccessFirst(_ =>
            {
                SqlBuilder.Insert(0, "WITH CommonTableExpression AS (");
                Append(")");

                StringBuilder outerSelectBuilder = new(),
                    innerSelectBuilder = new(),
                    groupByFilteringBuilder = new(),
                    onClauseBuilder = new();

                new EnumeratorProcessor<KeyValuePair<string, Type>>(AggregationKeys)
                    .AccessFirst(kv =>
                    {
                        outerSelectBuilder.Append($"GP.{kv.Key}");
                    })
                    .AccessRemaining(kv =>
                    {
                        outerSelectBuilder.AppendLine($", GP.{kv.Key}");
                    })
                    .AccessLast(() => outerSelectBuilder.Append(','))
                    .Execute();

                new EnumeratorProcessor<KeyValuePair<string, Type>>(GroupingKeys)
                    .AccessFirst(kv =>
                    {
                        outerSelectBuilder.Append($"GP.{kv.Key}");
                        innerSelectBuilder.Append($"{kv.Key}");
                        groupByFilteringBuilder.Append($"GROUP BY {kv.Key}");
                        onClauseBuilder.Append($"CTE.{kv.Key} = GP.{kv.Key}");
                    })
                    .AccessRemaining(kv =>
                    {
                        outerSelectBuilder.AppendLine($", GP.{kv.Key}");
                        innerSelectBuilder.AppendLine($", {kv.Key}");
                        groupByFilteringBuilder.AppendLine($", {kv.Key}");
                        onClauseBuilder.AppendLine(
                            $"AND CTE.{kv.Key} = GP.{kv.Key}");
                    })
                    .AccessLast(() => outerSelectBuilder.Append(','))
                    .Execute();

                outerSelectBuilder.Append("CTE.*");

                new EnumeratorProcessor<FormattableString>(SqlStatements[SqlStatement.SelectAggregate])
                    .AccessFirst(fs =>
                    {
                        if (innerSelectBuilder.Length > 0)
                        {
                            innerSelectBuilder.Append(',');
                        }

                        innerSelectBuilder.Append($" {fs}");
                    })
                    .AccessRemaining(fs =>
                    {
                        innerSelectBuilder.AppendLine($", {fs}");
                    })
                    .Execute();

                new EnumeratorProcessor<FormattableString>(SqlStatements[SqlStatement.Having])
                    .AccessFirst(fs =>
                    {
                        groupByFilteringBuilder.Append($" HAVING {fs}");
                    })
                    .AccessRemaining(fs =>
                    {
                        groupByFilteringBuilder.AppendLine($"AND {fs}");
                    })
                    .Execute();

                Append($@"
                    SELECT
                        {outerSelectBuilder}
                    FROM CommonTableExpression CTE
                    JOIN (
                        SELECT
                            {innerSelectBuilder}
                        FROM CommonTableExpression
                            {groupByFilteringBuilder}
                    ) GP
                    ON {onClauseBuilder};
                ");

                AppendLine();
            })
            .Execute();

    /// <summary>
    ///     Builds the ORDER BY clause of the SQL query by assembling sorting
    ///     conditions from the stored statements.
    /// </summary>
    private void SetOrderBy()
        => new EnumeratorProcessor<FormattableString>(SqlStatements[SqlStatement.OrderBy])
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
        => new EnumeratorProcessor<FormattableString>(SqlStatements[SqlStatement.Limit])
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
        => new EnumeratorProcessor<FormattableString>(SqlStatements[SqlStatement.Offset])
            .AccessFirst(fs =>
            {
                Append("OFFSET");
                AppendLine($"{fs}");
                AppendLine();
            })
            .Execute();
}
