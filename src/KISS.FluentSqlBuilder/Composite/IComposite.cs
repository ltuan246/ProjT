namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public interface IComposite
{
    /// <summary>
    ///     Gets the final SQL query string generated from the query builder.
    ///     This property combines all SQL statements in the correct order and applies
    ///     any necessary formatting.
    /// </summary>
    string Sql { get; }

    /// <summary>
    ///     Gets the collection of dynamic parameters used in the SQL query.
    ///     These parameters are used for safe parameter binding and preventing SQL injection.
    /// </summary>
    DynamicParameters Parameters { get; }

    /// <summary>
    ///     Gets the StringBuilder instance used to construct the SQL query.
    ///     This builder accumulates SQL statements and clauses during query construction.
    /// </summary>
    StringBuilder SqlBuilder { get; }

    /// <summary>
    ///     Gets the SQL formatter instance used for custom string formatting
    ///     and parameter handling in SQL queries.
    /// </summary>
    SqlFormatter SqlFormatting { get; }

    /// <summary>
    ///     This collection maintains separate lists for different SQL clauses
    ///     (SELECT, FROM, JOIN, etc.) to ensure proper query construction.
    /// </summary>
    Dictionary<SqlStatement, List<string>> SqlStatements { get; }

    /// <summary>
    ///     Gets the dictionary that maps table types to their SQL aliases.
    ///     This collection is used to maintain consistent table aliases
    ///     throughout the query construction process.
    /// </summary>
    Dictionary<Type, string> TableAliases { get; }

    /// <summary>
    ///     CurrentEntryExParameter.
    /// </summary>
    Type InEntityType { get; }

    /// <summary>
    ///     CurrentEntryExParameter.
    /// </summary>
    Type OutEntityType { get; }

    /// <summary>
    ///     CurrentEntryExParameter.
    /// </summary>
    Type OutEntitiesType { get; }

    /// <summary>
    ///     CurrentEntryExParameter.
    /// </summary>
    ParameterExpression InEntriesExParameter { get; }

    /// <summary>
    ///     CurrentEntryExParameter.
    /// </summary>
    ParameterExpression InEntriesExVariable { get; }

    /// <summary>
    ///     IndexerExVariable.
    /// </summary>
    ParameterExpression IndexerExVariable { get; }

    /// <summary>
    ///     CurrentEntryExParameter.
    /// </summary>
    ParameterExpression OutEntitiesExVariable { get; }

    /// <summary>
    ///     CurrentEntryExParameter.
    /// </summary>
    ParameterExpression CurrentEntryExVariable { get; }

    /// <summary>
    ///     CurrentEntryExParameter.
    /// </summary>
    ParameterExpression CurrentEntityExVariable { get; }

    /// <summary>
    ///     JoinRows.
    /// </summary>
    List<Expression> JoinRows { get; }

    /// <summary>
    ///     CurrentEntryExParameter.
    /// </summary>
    BlockExpression Block { get; }

    /// <summary>
    ///     Retrieves or creates a table alias for the specified type in the query context.
    ///     This method ensures consistent alias usage throughout the query construction.
    /// </summary>
    /// <param name="type">The type for which to retrieve or generate a table alias.</param>
    /// <returns>
    ///     A string representing the alias associated with the specified type.
    ///     If no alias exists, a new one is generated and stored.
    /// </returns>
    string GetAliasMapping(Type type);
}
