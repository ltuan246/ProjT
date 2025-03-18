namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     CompositeQuery.
/// </summary>
public sealed partial class CompositeQuery
{
    /// <summary>
    ///     Appends a formatted string to the <see cref="SqlBuilder" />.
    /// </summary>
    /// <param name="value">The string to append.</param>
    private void Append(string value)
        => SqlBuilder.Append(value);

    /// <summary>
    ///     Appends a new line to the string being built.
    /// </summary>
    /// <param name="value">The string to append.</param>
    /// <param name="indent">Refers to adding spaces at the beginning of lines of text.</param>
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
    ///     Retrieves the alias mapped to the specified <see cref="Type" /> in the query context,
    ///     or creates a new alias if none exists.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> for which to retrieve or generate a table alias.</param>
    /// <returns>
    ///     A <see cref="string" /> representing the alias associated with the specified <paramref name="type" />.
    /// </returns>
    public string GetAliasMapping(Type type)
    {
        if (!TableAliases.TryGetValue(type, out var tableAlias))
        {
            const string defaultTableAlias = "Extend";

            // Generate a new alias based on the default alias prefix and current alias count.
            tableAlias = $"{defaultTableAlias}{TableAliases.Count}";

            // Store the new alias in the dictionary for future reference.
            TableAliases.Add(type, tableAlias);
        }

        // Return the alias (existing or newly created) for the specified type.
        return tableAlias;
    }

    /// <summary>
    ///     Builds the SQL query string by assembling query clauses from stored statements.
    ///     Uses a <see cref="StringBuilder" /> (via inherited or composed behavior) to construct the query incrementally.
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

    private void SetSelect()
    {
        // Retrieves an enumerator for SELECT statements stored in SqlStatements, a presumed collection.
        using var itor = SqlStatements[SqlStatement.Select].GetEnumerator();

        // Checks if there are any SELECT statements to process.
        if (itor.MoveNext())
        {
            // Begins the query with the "SELECT" keyword on a new line.
            Append("SELECT");
            // Appends the first SELECT statement, typically a column or expression.
            AppendLine($"{itor.Current}");

            // Iterates through remaining SELECT statements, prefixing each with a comma for proper SQL syntax.
            while (itor.MoveNext())
            {
                AppendLine($", {itor.Current}");
            }

            // Adds an empty line to finalize the SELECT clause in the SQL string.
            AppendLine();
        }
    }

    private void SetFrom()
    {
        (var table, var alias) = TableAliases.First();
        // Appends the "FROM" clause, using the first table name and its alias from TableAliases, a presumed collection.
        Append("FROM");
        AppendLine($"{table.Name}s AS {alias}");
        // Adds an empty line to separate clauses in the SQL string.
        AppendLine();
    }

    private void SetJoin()
    {
        // Retrieves an enumerator for JOIN conditions stored in SqlStatements.
        using var itor = SqlStatements[SqlStatement.Join].GetEnumerator();

        // Checks if there are any JOIN conditions to process.
        if (itor.MoveNext())
        {
            // Appends the first JOIN condition.
            Append($"{itor.Current}");

            // Iterates through remaining JOIN conditions, combining them with "AND" for logical conjunction.
            while (itor.MoveNext())
            {
                AppendLine($"{itor.Current}");
            }

            // Adds an empty line to finalize the JOIN clause in the SQL string.
            AppendLine();
        }
    }

    private void SetWhere()
    {
        // Retrieves an enumerator for WHERE conditions stored in SqlStatements.
        using var itor = SqlStatements[SqlStatement.Where].GetEnumerator();

        // Checks if there are any WHERE conditions to process.
        if (itor.MoveNext())
        {
            // Begins the WHERE clause with the keyword on a new line.
            Append("WHERE");

            // Appends the first WHERE condition.
            AppendLine($"{itor.Current}");

            // Iterates through remaining WHERE conditions, combining them with "AND" for logical conjunction.
            while (itor.MoveNext())
            {
                AppendLine($"AND {itor.Current}");
            }

            // Adds an empty line to finalize the WHERE clause in the SQL string.
            AppendLine();
        }
    }

    private void SetGroupBy()
    {
        // Retrieves an enumerator for GROUP BY conditions stored in SqlStatements.
        using var itor = SqlStatements[SqlStatement.GroupBy].GetEnumerator();

        if (itor.MoveNext())
        {
            SqlBuilder.Insert(0, "WITH CommonTableExpression AS (");
            Append(")");

            StringBuilder outerSelectBuilder = new(),
                innerSelectBuilder = new(),
                groupByFilteringBuilder = new(),
                onClauseBuilder = new();

            using var aggregatingItor = AggregationKeys.GetEnumerator();

            if (aggregatingItor.MoveNext())
            {
                outerSelectBuilder.Append($"GP.{aggregatingItor.Current.Key}");

                while (aggregatingItor.MoveNext())
                {
                    outerSelectBuilder.AppendLine($", GP.{aggregatingItor.Current.Key}");
                }

                outerSelectBuilder.Append(',');
            }

            outerSelectBuilder.Append("CTE.*");

            using var groupingItor = GroupingKeys.GetEnumerator();

            if (groupingItor.MoveNext())
            {
                innerSelectBuilder.Append($"{groupingItor.Current.Key}");
                groupByFilteringBuilder.Append($"GROUP BY {groupingItor.Current.Key}");
                onClauseBuilder.Append($"CTE.{groupingItor.Current.Key} = GP.{groupingItor.Current.Key}");

                while (groupingItor.MoveNext())
                {
                    innerSelectBuilder.AppendLine($", {groupingItor.Current.Key}");
                    groupByFilteringBuilder.AppendLine($", {groupingItor.Current.Key}");
                    onClauseBuilder.AppendLine($"AND CTE.{groupingItor.Current.Key} = GP.{groupingItor.Current.Key}");
                }

                innerSelectBuilder.Append(',');
            }

            // Retrieves an enumerator for HAVING conditions stored in SqlStatements.
            using var havingItor = SqlStatements[SqlStatement.Having].GetEnumerator();

            // Checks if there are any HAVING conditions to process.
            if (havingItor.MoveNext())
            {
                // Begins the HAVING clause with the keyword on a new line.
                groupByFilteringBuilder.Append($" HAVING {havingItor.Current}");

                // Iterates through remaining HAVING conditions, combining them with "AND" for logical conjunction.
                while (havingItor.MoveNext())
                {
                    groupByFilteringBuilder.AppendLine($"AND {havingItor.Current}");
                }
            }

            // Retrieves an enumerator for HAVING conditions stored in SqlStatements.
            using var aggregateItor = SqlStatements[SqlStatement.SelectAggregate].GetEnumerator();

            // Checks if there are any HAVING conditions to process.
            if (aggregateItor.MoveNext())
            {
                // Begins the HAVING clause with the keyword on a new line.
                innerSelectBuilder.Append($" {aggregateItor.Current}");

                // Iterates through remaining HAVING conditions, combining them with "AND" for logical conjunction.
                while (aggregateItor.MoveNext())
                {
                    innerSelectBuilder.AppendLine($", {aggregateItor.Current}");
                }
            }

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
        }
    }

    private void SetOrderBy()
    {
        // Retrieves an enumerator for ORDER BY conditions stored in SqlStatements.
        using var itor = SqlStatements[SqlStatement.OrderBy].GetEnumerator();

        // Checks if there are any ORDER BY conditions to process.
        if (itor.MoveNext())
        {
            // Begins the ORDER BY clause with the keyword on a new line.
            Append("ORDER BY");
            // Appends the first ORDER BY condition.
            AppendLine($"{itor.Current}", true);

            // Iterates through remaining ORDER BY conditions, combining them with "," for logical conjunction.
            while (itor.MoveNext())
            {
                AppendLine($", {itor.Current}", true);
            }

            // Adds an empty line to finalize the ORDER BY clause in the SQL string.
            AppendLine();
        }
    }

    private void SetLimit()
    {
        // Retrieves an enumerator for LIMIT conditions stored in SqlStatements.
        using var itor = SqlStatements[SqlStatement.Limit].GetEnumerator();

        // Checks if there are any LIMIT conditions to process.
        if (itor.MoveNext())
        {
            // Begins the LIMIT clause with the keyword on a new line.
            Append($"LIMIT {itor.Current}");
            // Adds an empty line to finalize the ORDER BY clause in the SQL string.
            AppendLine();
        }
    }

    private void SetOffset()
    {
        // Retrieves an enumerator for OFFSET conditions stored in SqlStatements.
        using var itor = SqlStatements[SqlStatement.Offset].GetEnumerator();

        // Checks if there are any OFFSET conditions to process.
        if (itor.MoveNext())
        {
            // Begins the OFFSET clause with the keyword on a new line.
            Append($"OFFSET {itor.Current}");
            // Adds an empty line to finalize the ORDER BY clause in the SQL string.
            AppendLine();
        }
    }
}
