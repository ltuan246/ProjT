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
    {
        using var itor = SqlStatements[SqlStatement.Select].GetEnumerator();

        if (itor.MoveNext())
        {
            Append("SELECT");
            AppendLine($"{itor.Current}");

            while (itor.MoveNext())
            {
                AppendLine($", {itor.Current}");
            }

            AppendLine();
        }
    }

    /// <summary>
    ///     Builds the FROM clause of the SQL query by specifying the main table
    ///     and its alias from the table aliases collection.
    /// </summary>
    private void SetFrom()
    {
        var (table, alias) = TableAliases.First();
        Append("FROM");
        AppendLine($"{table.Name}s AS {alias}");
        AppendLine();
    }

    /// <summary>
    ///     Builds the JOIN clauses of the SQL query by assembling join conditions
    ///     from the stored statements.
    /// </summary>
    private void SetJoin()
    {
        using var itor = SqlStatements[SqlStatement.Join].GetEnumerator();

        if (itor.MoveNext())
        {
            Append($"{itor.Current}");

            while (itor.MoveNext())
            {
                AppendLine($"{itor.Current}");
            }

            AppendLine();
        }
    }

    /// <summary>
    ///     Builds the WHERE clause of the SQL query by assembling conditions
    ///     from the stored statements.
    /// </summary>
    private void SetWhere()
    {
        using var itor = SqlStatements[SqlStatement.Where].GetEnumerator();

        if (itor.MoveNext())
        {
            Append("WHERE");
            AppendLine($"{itor.Current}");

            while (itor.MoveNext())
            {
                AppendLine($"AND {itor.Current}");
            }

            AppendLine();
        }
    }

    /// <summary>
    ///     Builds the GROUP BY clause of the SQL query, including handling of
    ///     Common Table Expressions (CTEs) for complex grouping scenarios.
    /// </summary>
    private void SetGroupBy()
    {
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

            using var groupingItor = GroupingKeys.GetEnumerator();

            if (groupingItor.MoveNext())
            {
                outerSelectBuilder.Append($"GP.{groupingItor.Current.Key}");
                innerSelectBuilder.Append($"{groupingItor.Current.Key}");
                groupByFilteringBuilder.Append($"GROUP BY {groupingItor.Current.Key}");
                onClauseBuilder.Append($"CTE.{groupingItor.Current.Key} = GP.{groupingItor.Current.Key}");

                while (groupingItor.MoveNext())
                {
                    outerSelectBuilder.AppendLine($", GP.{groupingItor.Current.Key}");
                    innerSelectBuilder.AppendLine($", {groupingItor.Current.Key}");
                    groupByFilteringBuilder.AppendLine($", {groupingItor.Current.Key}");
                    onClauseBuilder.AppendLine($"AND CTE.{groupingItor.Current.Key} = GP.{groupingItor.Current.Key}");
                }

                outerSelectBuilder.Append(',');
            }

            outerSelectBuilder.Append("CTE.*");

            using var aggregateItor = SqlStatements[SqlStatement.SelectAggregate].GetEnumerator();

            if (aggregateItor.MoveNext())
            {
                if (innerSelectBuilder.Length > 0)
                {
                    innerSelectBuilder.Append(',');
                }

                innerSelectBuilder.Append($" {aggregateItor.Current}");

                while (aggregateItor.MoveNext())
                {
                    innerSelectBuilder.AppendLine($", {aggregateItor.Current}");
                }
            }

            using var havingItor = SqlStatements[SqlStatement.Having].GetEnumerator();

            if (havingItor.MoveNext())
            {
                groupByFilteringBuilder.Append($" HAVING {havingItor.Current}");

                while (havingItor.MoveNext())
                {
                    groupByFilteringBuilder.AppendLine($"AND {havingItor.Current}");
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

    /// <summary>
    ///     Builds the ORDER BY clause of the SQL query by assembling sorting
    ///     conditions from the stored statements.
    /// </summary>
    private void SetOrderBy()
    {
        using var itor = SqlStatements[SqlStatement.OrderBy].GetEnumerator();

        if (itor.MoveNext())
        {
            Append("ORDER BY");
            AppendLine($"{itor.Current}");

            while (itor.MoveNext())
            {
                AppendLine($", {itor.Current}");
            }

            AppendLine();
        }
    }

    /// <summary>
    ///     Builds the LIMIT clause of the SQL query by adding the limit
    ///     value from the stored statements.
    /// </summary>
    private void SetLimit()
    {
        using var itor = SqlStatements[SqlStatement.Limit].GetEnumerator();

        if (itor.MoveNext())
        {
            Append("LIMIT");
            AppendLine($"{itor.Current}");
            AppendLine();
        }
    }

    /// <summary>
    ///     Builds the OFFSET clause of the SQL query by adding the offset
    ///     value from the stored statements.
    /// </summary>
    private void SetOffset()
    {
        using var itor = SqlStatements[SqlStatement.Offset].GetEnumerator();

        if (itor.MoveNext())
        {
            Append("OFFSET");
            AppendLine($"{itor.Current}");
            AppendLine();
        }
    }
}
