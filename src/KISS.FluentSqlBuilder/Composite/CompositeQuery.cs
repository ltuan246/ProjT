namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     Implements <see cref="ICompositeQueryOperations" /> to provide query execution capabilities.
/// </summary>
/// <param name="connection">The <see cref="DbConnection" /> used to execute the query.</param>
public sealed partial class CompositeQuery(DbConnection connection) : ICompositeQueryOperations
{
    /// <summary>
    ///     Gets the database connection used for executing SQL queries.
    ///     Initialized via the constructor and remains constant throughout the instance's lifetime.
    /// </summary>
    private DbConnection Connection { get; } = connection;

    /// <summary>
    ///     Executes the constructed SQL query against the database and returns the results as a list of the specified type.
    /// </summary>
    /// <typeparam name="TReturn">The type of objects to return, representing the query result rows.</typeparam>
    /// <returns>A list of <typeparamref name="TReturn" /> objects retrieved based on the query conditions.</returns>
    public List<TReturn> GetList<TReturn>()
    {
        // Executes the SQL query using the Connection, passing the constructed Sql string and Parameters (presumed properties).
        // Casts the result to a list of dictionaries for flexible row data access.
        var dtRows = Connection.Query(Sql, Parameters)
            .Cast<IDictionary<string, object>>()
            .ToList();

        // Processes the raw data rows into a typed list of TReturn objects using a dynamic expression.
        // The lambda defines how to add each row (as CurrentEntityVariable) to the output list (OutputCollectionVariable).
        var res = JoinRowProcessors.Count != 0 ? DictProcess<TReturn>(dtRows) : SimpleProcess<TReturn>(dtRows); // Adds the current entity to the list.

        // Returns the populated list of query results.
        return res;
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
        // SetGroupBy();
        // SetHaving();
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
