namespace KISS.FluentSqlBuilder.Decorators.SelectDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TIn">The type representing the database record set.</typeparam>
/// <typeparam name="TOut">The combined type to return.</typeparam>
public sealed partial record SelectDecorator<TIn, TOut> : IComposite
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectDecorator{TIn, TOut}"/> class.
    /// </summary>
    /// <param name="inner">innerComposite.</param>
    public SelectDecorator(IComposite inner)
    {
        InEntityType = inner.InEntityType;
        OutEntityType = inner.OutEntityType;
        OutEntitiesType = inner.OutEntitiesType;
        Parameters = inner.Parameters;
        SqlStatements = inner.SqlStatements;
        TableAliases = inner.TableAliases;
        SqlFormatting = inner.SqlFormatting;

        InEntriesExParameter = inner.InEntriesExParameter;
        InEntriesExVariable = inner.InEntriesExVariable;
        OutEntitiesExVariable = inner.OutEntitiesExVariable;
        CurrentEntryExVariable = inner.CurrentEntryExVariable;
        CurrentEntityExVariable = inner.CurrentEntityExVariable;
    }

    /// <inheritdoc />
    public Type InEntityType { get; init; }

    /// <inheritdoc />
    public Type OutEntityType { get; init; }

    /// <inheritdoc />
    public Type OutEntitiesType { get; init; }

    /// <inheritdoc />
    public string Sql { get { return SetQueries(); } }

    /// <inheritdoc />
    public DynamicParameters Parameters { get; init; }

    /// <inheritdoc />
    public Dictionary<SqlStatement, List<string>> SqlStatements { get; init; }

    /// <inheritdoc />
    public Dictionary<Type, string> TableAliases { get; init; }
}