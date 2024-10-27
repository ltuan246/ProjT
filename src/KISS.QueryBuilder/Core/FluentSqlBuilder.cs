namespace KISS.QueryBuilder.Core;

/// <summary>
///     A class that defines the fluent SQL builder type.
///     The core <see cref="FluentSqlBuilder{TRecordset}" /> partial class.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public partial class FluentSqlBuilder<TRecordset>
{
    /// <summary>
    ///      Initializes a new instance of the <see cref="FluentSqlBuilder{TRecordset}"/> class.
    /// </summary>
    public FluentSqlBuilder()
    {
        var recType = typeof(TRecordset);
        var tableAlias = GetTableAlias(recType);
        SqlBuilder = new();
        Append($"{ClauseConstants.Select} * FROM {recType.Name}s {tableAlias} ");
    }

    /// <inheritdoc/>
    public string Sql
        => SqlBuilder.ToString();

    /// <inheritdoc />
    public DynamicParameters Parameters
        => SqlFormat.Parameters;

    /// <summary>
    ///     The connection to a database.
    /// </summary>
    public required DbConnection Connection { get; init; }

    private StringBuilder SqlBuilder { get; init; }

    private SqlFormatter SqlFormat { get; } = new();

    private Type RootTable { get; } = typeof(TRecordset);

    private ParameterExpression ReturnParam { get; } =
        Expression.Parameter(typeof(TRecordset), ClauseConstants.DefaultTableAlias);

    /// <summary>
    ///     The expressions are both ordered and intended for use in an Expression.Block.
    /// </summary>
    private List<(ParameterExpression Parameter, Expression Expr)> BlockMapSequence { get; } = [];

    /// <summary>
    ///     Define parameters for accumulated in the Aggregate.
    /// </summary>
    private ParameterExpression AccumulatedParam { get; } =
        Expression.Parameter(typeof(TRecordset), "acc");

    /// <summary>
    ///     Define parameters for current in the Aggregate.
    /// </summary>
    private ParameterExpression CurrentParam { get; } =
        Expression.Parameter(typeof(TRecordset), "cur");

    /// <summary>
    ///     The expressions are both ordered and intended for use in an Expression.Block.
    /// </summary>
    private List<Expression> BlockAggregateSequence { get; } = [];

    private Dictionary<Type, string> TableAliasesMap { get; } = new()
    {
        [typeof(TRecordset)] = $"{ClauseConstants.DefaultTableAlias}{0}"
    };

    private List<ClauseAction> ClauseActions { get; set; } = [];

    /// <summary>
    ///     Use checks to know when to use Distinct.
    /// </summary>
    private bool HasDistinct { get; set; }

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    private bool HasOpenParentheses { get; set; }

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

    private Delegate CreatingMap()
    {
        // Define parameters for the lambda expression
        var parameters = BlockMapSequence.Select(e => e.Parameter).ToList();

        // Define multiple expressions (Access method).
        var methods = BlockMapSequence.Select(e => e.Expr).ToList();

        // Create block expression
        // The list of expressions is passed to Expression.Block as a parameter.
        // Expression.Block executes the expressions in order.
        // It returns the last expression in the block, which is rParam (the TEntity).
        var block = Expression.Block([.. methods, ReturnParam]);

        // Dynamically constructs the corresponding Func type using reflection.
        List<Type> types = [.. TableAliasesMap.Keys, typeof(TRecordset)];
        var funcType = typeof(Func<>).Assembly
            .GetType($"System.Func`{types.Count}")! // Func type has an additional return type
            .MakeGenericType([.. types]);

        // Create the lambda expression
        var lambda = Expression.Lambda(funcType, block, [ReturnParam, .. parameters]);

        // Compile the expression tree into a delegate
        var map = lambda.Compile();

        return map;
    }

    private MethodInfo CreatingQuery()
    {
        List<Type> types = [.. TableAliasesMap.Keys, typeof(TRecordset)];
        var queryMethod = typeof(SqlMapper)
            .GetMethods()
            .First(m => m is { Name: "Query", IsGenericMethod: true }
                        && m.GetGenericArguments().Length == types.Count);

        var method = queryMethod.MakeGenericMethod([.. types]);

        return method;
    }
}
