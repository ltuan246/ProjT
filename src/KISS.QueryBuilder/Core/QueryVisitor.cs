namespace KISS.QueryBuilder.Core;

/// <summary>
///     A class that defines the fluent SQL builder type.
///     The core <see cref="QueryVisitor" /> partial class.
/// </summary>
/// <param name="recordset">The type representing the database record set.</param>
internal sealed partial class QueryVisitor(Type recordset) : ISqlBuilder
{
    private Type RootTable { get; } = recordset;

    private StringBuilder SqlBuilder { get; } = new();

    private SqlFormatter SqlFormat { get; } = new();

    private Dictionary<Type, string> TableAliasesMap { get; } = new()
    {
        { recordset, $"{ClauseConstants.DefaultTableAlias}{0}" }
    };

    private ParameterExpression ReturnParam { get; } =
        Expression.Variable(recordset, "currentRecordset");

    /// <summary>
    ///     The expressions are both ordered and intended for use in an Expression.Block.
    /// </summary>
    private List<(ParameterExpression Parameter, Expression Expr)> BlockMapSequence { get; } = [];

    private List<ClauseAction> ClauseActions { get; } = [];

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    private bool HasOpenParentheses { get; set; }

    /// <inheritdoc />
    public string Sql
        => SqlBuilder.ToString();

    /// <inheritdoc />
    public DynamicParameters Parameters
        => SqlFormat.Parameters;

    private void OpenParentheses()
    {
        HasOpenParentheses = true;
        SqlBuilder.Append(ClauseConstants.OpenParenthesis);
    }

    private void CloseParentheses()
    {
        if (!HasOpenParentheses)
        {
            return;
        }

        HasOpenParentheses = false;
        SqlBuilder.Append(ClauseConstants.CloseParenthesis);
    }

    private string GetTableAlias(Type type)
    {
        if (!TableAliasesMap.TryGetValue(type, out var tableAlias))
        {
            tableAlias = $"{ClauseConstants.DefaultTableAlias}{TableAliasesMap.Count}";
            TableAliasesMap.Add(type, tableAlias);
        }

        return tableAlias;
    }

    /// <summary>
    ///     Generates a delegate that maps records into a dictionary based on a unique identifier (e.g., `Id`).
    ///     If a record with the given identifier does not already exist in the dictionary, it will be added.
    ///     This delegate is later used by Dapper as a custom mapping function for database records.
    /// </summary>
    /// <param name="dict">A dictionary used to store and retrieve records of type `TRecordset` by ID.</param>
    /// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
    /// <returns>A compiled delegate (Func) that performs dictionary lookups and conditionally adds new records.</returns>
    public Delegate BuildMapRecordset<TRecordset>(Dictionary<string, TRecordset> dict)
    {
        // Get the type of the dictionary parameter to use in reflection
        var dictType = dict.GetType();

        // Define parameters for the lambda expression
        var parameters = BlockMapSequence.Select(e => e.Parameter).ToList();

        // Define multiple expressions (Access method).
        var methods = BlockMapSequence.Select(e => e.Expr).ToList();

        // Create a constant expression representing the dictionary, so it can be used in the expression tree
        var dictParam = Expression.Constant(dict);

        // Define a parameter expression for the lambda function, representing a single record of type TRecordset
        var recordsetParam = Expression.Parameter(typeof(TRecordset), "recordset");

        // Create a call to the dictionary's TryGetValue method to check if a record exists by its Id property
        var tryGetValueCall = Expression.Call(
            dictParam,
            dictType.GetMethod("TryGetValue")!,
            Expression.Property(recordsetParam, "Id"),
            ReturnParam);

        // Define a block to add a new record to the dictionary if TryGetValue fails
        var addNewToDictBlock = Expression.Block(
            Expression.Assign(ReturnParam, recordsetParam),
            Expression.Call(
                dictParam,
                dictType.GetMethod("Add")!,
                Expression.Property(recordsetParam, "Id"),
                recordsetParam));

        // If the record is not in the dictionary (TryGetValue returns false), add it using the block defined above
        var ifNotInDict = Expression.IfThen(Expression.IsFalse(tryGetValueCall), addNewToDictBlock);

        // Combine expressions into a block: check dictionary, execute additional methods, and return result
        var block = Expression.Block([ReturnParam], [ifNotInDict, .. methods, ReturnParam]);

        // Dynamically constructs the corresponding Func type using reflection.
        List<Type> types = [.. TableAliasesMap.Keys, typeof(TRecordset)];
        var funcType = typeof(Func<>).Assembly
            .GetType($"System.Func`{types.Count}")! // Func type has an additional return type
            .MakeGenericType([.. types]);

        // Create the lambda expression
        var lambda = Expression.Lambda(funcType, block, [recordsetParam, .. parameters]);

        // Compile the expression tree into a delegate
        var map = lambda.Compile();

        return map;
    }

    /// <summary>
    ///     Locates the appropriate generic `Query` method from Dapper's `SqlMapper` type, which allows custom mapping.
    ///     Constructs a version of this method with the correct generic type arguments based on `TRecordset` and
    ///     additional types provided by `TableAliasesMap`.
    /// </summary>
    /// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
    /// <returns>A MethodInfo object representing the generic `Query` method tailored for the type mappings.</returns>
    public MethodInfo CreatingQuery<TRecordset>()
    {
        // Create a list of parameter types for the query method
        List<Type> types = [.. TableAliasesMap.Keys, typeof(TRecordset)];

        // Find a generic method named "Query" in SqlMapper with the correct number of generic arguments
        var queryMethod = typeof(SqlMapper)
            .GetMethods()
            .First(m => m is { Name: "Query", IsGenericMethod: true }
                        && m.GetGenericArguments().Length == types.Count);

        // Create a specialized generic method by applying types as generic parameters
        var method = queryMethod.MakeGenericMethod([.. types]);

        return method;
    }
}
