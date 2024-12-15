namespace KISS.FluentSqlBuilder.Composites;

/// <summary>
///     A class that defines the fluent SQL builder type.
/// </summary>
public sealed partial class CompositeQuery
{
    /// <summary>
    ///     The type representing the database record set.
    /// </summary>
    public required Type SourceEntity { get; init; }

    /// <summary>
    ///     The type representing the database record set.
    /// </summary>
    public required ParameterExpression SourceParameter { get; init; }

    /// <summary>
    ///     The type of the record.
    /// </summary>
    public required Type RetrieveEntity { get; init; }

    /// <summary>
    ///     The type of the record.
    /// </summary>
    public required ParameterExpression RetrieveParameter { get; init; }

    private List<PropertyInfo> TargetProperties
        => RetrieveEntity.GetProperties().Where(p => p.CanWrite).ToList();

    /// <summary>
    ///     The expressions are both ordered and intended for use in an Expression.Block.
    /// </summary>
    private List<(ParameterExpression Parameter, Expression Expr)> BlockMapSequence { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> SelectComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> SelectAsAliasComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> SelectFromComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<(Type Recordset, Expression LeftKeySelector, Expression RightKeySelector)> JoinComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> WhereComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> GroupByComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> HavingComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> OrderByComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> LimitComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> OffsetComponents { get; } = [];

    /// <summary>
    ///     Gets the query components.
    /// </summary>
    public void SetQueries()
    {
        SetSelect();
        SetFrom();
        SetJoin();
        SetWhere();
        SetGroupBy();
        SetHaving();
        SetOrderBy();
        SetLimit();
        SetOffset();
    }

    /// <summary>
    ///     Configures a mapping between a parent entity and a related entity.
    /// </summary>
    /// <param name="mapSelector">
    ///     An expression that specifies the property in the parent entity where the collection
    ///     of related entities is stored. This is typically a navigation property.
    /// </param>
    /// <typeparam name="TReturn">The type of the parent entity.</typeparam>
    /// <typeparam name="TRelation">The type of the related entities in the collection.</typeparam>
    /// <exception cref="NotSupportedException">
    ///     Thrown if the provided <paramref name="mapSelector" /> is not a valid member expression.
    /// </exception>
    public void SetMap<TReturn, TRelation>(Expression<Func<TReturn, TRelation?>> mapSelector)
    {
        switch (mapSelector.Body)
        {
            case MemberExpression memberExpression:
                {
                    var relationParam = Expression.Parameter(typeof(TRelation), GetAliasMapping(typeof(TRelation)));
                    var currentProperty = Expression.Property(RetrieveParameter, memberExpression.Member.Name);
                    var assignExpression = Expression.Assign(currentProperty, relationParam);
                    BlockMapSequence.Add((relationParam, assignExpression));

                    break;
                }

            default:
                throw new NotSupportedException("Expression not supported.");
        }
    }

    /// <summary>
    ///     Configures a mapping between a parent entity and a collection of related entities.
    /// </summary>
    /// <param name="mapSelector">
    ///     An expression that specifies the property in the parent entity where the collection
    ///     of related entities is stored. This is typically a navigation property.
    /// </param>
    /// <typeparam name="TReturn">The type of the parent entity.</typeparam>
    /// <typeparam name="TRelation">The type of the related entities in the collection.</typeparam>
    /// <exception cref="NotSupportedException">
    ///     Thrown if the provided <paramref name="mapSelector" /> is not a valid member expression.
    /// </exception>
    public void SetMap<TReturn, TRelation>(Expression<Func<TReturn, List<TRelation>?>> mapSelector)
    {
        switch (mapSelector.Body)
        {
            case MemberExpression memberExpression:
                {
                    var relationParam = Expression.Parameter(typeof(TRelation), GetAliasMapping(typeof(TRelation)));
                    var currentProperty = Expression.Property(RetrieveParameter, memberExpression.Member.Name);

                    var newList = Expression.IfThen(
                        Expression.Equal(currentProperty, Expression.Constant(null)),
                        Expression.Assign(currentProperty, Expression.New(typeof(List<TRelation>))));

                    // Add relation to mapSelector if it exists
                    var addToList = Expression.IfThen(
                        Expression.NotEqual(relationParam, Expression.Constant(null)),
                        Expression.Block(
                            newList,
                            Expression.Call(currentProperty, "Add", null, relationParam)));

                    BlockMapSequence.Add((relationParam, addToList));

                    break;
                }

            default:
                throw new NotSupportedException("Expression not supported.");
        }
    }

    /// <summary>
    ///     Creates a collection of <see cref="System.Linq.Expressions.MemberBinding" /> instances
    ///     that map properties from the source type to the target type.
    /// </summary>
    /// <returns>
    ///     An enumerable collection of <see cref="System.Linq.Expressions.MemberBinding" /> objects,
    ///     where each binding represents the assignment of a source property value to a target property.
    /// </returns>
    private IEnumerable<MemberBinding> CreateBindings()
    {
        foreach (var targetProperty in TargetProperties)
        {
            var sourceProperty = SourceEntity.GetProperty(targetProperty.Name);
            if (sourceProperty != null && sourceProperty.PropertyType == targetProperty.PropertyType)
            {
                var sourceValue = Expression.Property(SourceParameter, sourceProperty);
                var binding = Expression.Bind(targetProperty, sourceValue);
                yield return binding;
            }
        }
    }

    /// <summary>
    ///     Creates a mapping function that transforms an instance of <typeparamref name="TRecordset" />
    ///     into an instance of <typeparamref name="TReturn" />.
    ///     This is useful for dynamically mapping database record sets to strongly typed objects.
    /// </summary>
    /// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>
    ///     A function that accepts a single <typeparamref name="TRecordset" /> instance and returns
    ///     a transformed <typeparamref name="TReturn" /> instance.
    /// </returns>
    public Func<TRecordset, TReturn> CreateMap<TRecordset, TReturn>()
    {
        var bindings = CreateBindings();
        var initializer = Expression.MemberInit(Expression.New(RetrieveEntity), bindings);
        var lambda = Expression.Lambda<Func<TRecordset, TReturn>>(initializer, SourceParameter);
        return lambda.Compile();
    }

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <param name="connection">The database connections.</param>
    /// <typeparam name="TFirst">The first type in the recordset.</typeparam>
    /// <typeparam name="TSecond">The second type in the recordset.</typeparam>
    /// <typeparam name="TThird">The third type in the recordset.</typeparam>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    public List<TReturn> ToList<TFirst, TSecond, TThird, TReturn>(DbConnection connection)
    {
        Dictionary<string, TReturn> dict = [];

        // Get the type of the dictionary parameter to use in reflection
        var dictType = dict.GetType();

        // Create a constant expression representing the dictionary, so it can be used in the expression tree
        var dictConst = Expression.Constant(dict);

        var sourceParameter = Expression.Parameter(typeof(TFirst), "source");
        // var retrieveParameter = Expression.Variable(typeof(TReturn), "retrieve");

        // Create a call to the dictionary's TryGetValue method to check if a record exists by its Id property
        var tryGetCall = Expression.Call(
            dictConst,
            dictType.GetMethod("TryGetValue")!,
            Expression.Property(sourceParameter, "Id"),
            RetrieveParameter);

        var bindings = new List<MemberBinding>();

        foreach (var targetProperty in TargetProperties)
        {
            var sourceProperty = typeof(TFirst).GetProperty(targetProperty.Name);
            if (sourceProperty != null && sourceProperty.PropertyType == targetProperty.PropertyType)
            {
                var sourceValue = Expression.Property(sourceParameter, sourceProperty);
                var binding = Expression.Bind(targetProperty, sourceValue);
                bindings.Add(binding);
            }
        }

        var initializer = Expression.MemberInit(Expression.New(RetrieveEntity), bindings);

        // Define a block to add a new record to the dictionary if TryGetValue fails
        var addNewToDictBlock = Expression.Block(
            Expression.Assign(RetrieveParameter, initializer),
            Expression.Call(
                dictConst,
                dictType.GetMethod("Add")!,
                Expression.Property(sourceParameter, "Id"),
                RetrieveParameter));

        // If the record is not in the dictionary (TryGetValue returns false), add it using the block defined above
        var ifNotInDict = Expression.IfThen(Expression.IsFalse(tryGetCall), addNewToDictBlock);

        // Define parameters for the lambda expression
        var parameters = BlockMapSequence.Select(e => e.Parameter).ToList();

        // Define multiple expressions (Access method).
        var methods = BlockMapSequence.Select(e => e.Expr).ToList();

        // Combine expressions into a block: check dictionary, execute additional methods, and return result
        var block = Expression.Block([RetrieveParameter], [ifNotInDict, ..methods, RetrieveParameter]);

        // Create the lambda expression
        var lambda = Expression.Lambda<Func<TFirst, TSecond, TReturn>>(block, [sourceParameter, ..parameters]);

        var map = lambda.Compile();

        SetQueries();
        _ = connection
            .Query<TFirst, TSecond, TThird, TReturn>(
                Sql,
                (first, second, third) => map(first, second),
                Parameters)
            .ToList();

        return [.. dict.Values];
    }
}
