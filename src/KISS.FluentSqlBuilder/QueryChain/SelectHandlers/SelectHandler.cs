namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     A handler for processing SELECT clauses in a query chain.
///     This class is responsible for generating SQL SELECT statements and mapping
///     database results to strongly-typed objects.
/// </summary>
/// <typeparam name="TSource">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record SelectHandler<TSource, TReturn> : QueryHandler, ISelectHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectHandler{TSource, TReturn}"/> class.
    /// </summary>
    public SelectHandler()
        : base(SqlStatement.Select)
    {
        CurrentEntityExVariable = Expression.Variable(OutEntityType, "CurrentEntityExVariable");
        OutEntitiesExVariable = Expression.Variable(OutEntitiesType, "OutEntitiesExVariable");
    }

    /// <summary>
    ///     Gets the type representing the database record set.
    ///     This type defines the structure of the data being queried from the database.
    /// </summary>
    public Type InEntityType { get; } = typeof(TSource);

    /// <summary>
    ///     Gets the combined type to return.
    ///     This type defines the structure of the object that will be created
    ///     from the query results.
    /// </summary>
    public Type OutEntityType { get; } = typeof(TReturn);

    private Type OutEntitiesType { get; } = typeof(List<TReturn>);

    /// <summary>
    /// CurrentEntityExVariable.
    /// </summary>
    public ParameterExpression CurrentEntityExVariable { get; init; }

    /// <summary>
    /// OutEntitiesExVariable.
    /// </summary>
    public ParameterExpression OutEntitiesExVariable { get; init; }

    /// <summary>
    /// InitializeEntityIfKeyMissing.
    /// </summary>
    public ConditionalExpression InitializeEntityIfKeyMissing
        => Expression.IfThen(
            Expression.Constant(true),
            Expression.Block(
                [], // Ensures variables is scoped for this operation
                    // Applies the row processor to construct or modify the entity.
                Composite.InitializeTargetValueBlock(CurrentEntityExVariable, Composite.CurrentEntryExParameter, InEntityType, OutEntityType),
                // Adds the processed entity to the dictionary with its key.
                Expression.Call(
                    OutEntitiesExVariable, // Calls the Add method on the output list.
                    OutEntitiesType.GetMethod("Add")!, // Retrieves the Add method via reflection.
                    CurrentEntityExVariable)));

    /// <summary>
    ///     Processes the SELECT clause by generating SQL statements for selecting
    ///     columns from the source entity and mapping them to the return type.
    /// </summary>
    protected override void Process()
    {
        var alias = Composite.GetAliasMapping(InEntityType);
        var sourceProperties = InEntityType.GetProperties()
            .Where(p => p.CanWrite)
            .Select(p => $"{alias}.{p.Name} AS {alias}_{p.Name}")
            .ToList();

        Composite.SqlStatements[SqlStatement.Select].Add(string.Join(", ", sourceProperties));
    }
}
