namespace KISS.QueryBuilder.Core;

/// <summary>
///     A class that defines the fluent SQL builder type. The core <see cref="FluentSqlBuilder{TEntity}" /> partial class.
/// </summary>
/// <typeparam name="TEntity">The type of results to return.</typeparam>
public sealed partial class FluentSqlBuilder<TEntity>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FluentSqlBuilder{TEntity}" /> class.
    /// </summary>
    public FluentSqlBuilder()
    {
        SqlBuilder = new StringBuilder();

        var entity = typeof(TEntity);
        var table = entity.Name;

        Append($"SELECT {TemporaryColumnsTemplate} FROM {table}s {DefaultEntityAliasTemplate}");
    }

    private StringBuilder SqlBuilder { get; }

    private SqlFormatter SqlFormat { get; } = new();

    private List<Type> JoiningTables { get; } = [];

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    private bool HasOpenParentheses { get; set; }

    private void OpenParentheses()
    {
        HasOpenParentheses = true;
        SqlBuilder.Append(OpenParenthesis);
    }

    private void CloseParentheses()
    {
        if (!HasOpenParentheses)
        {
            return;
        }

        HasOpenParentheses = false;
        SqlBuilder.Append(CloseParenthesis);
    }

    private void AddCommaSeparated()
        => SqlBuilder.Append(Comma);

    private void Map()
    {
        // Define parameters for the lambda expression
        var rParam = Expression.Parameter(typeof(TEntity), "r");
        var parameters = JoiningTables
            .Select(table => Expression.Parameter(table, "e"))
            .ToList();

        // Store multiple expressions (Access method).
        var methods = JoiningTables
            .Select(table =>
            {
                // Create expressions for setting the properties
                // e.A = r0;
                // var assignProperty = Expression.Property(rParam, string.Empty);
                // var assign = Expression.Assign(assignProperty, r0Param);

                // e.B.Add(r1);
                var property = Expression.Property(rParam, string.Empty);
                var addMethod = typeof(List<>).GetMethod("Add")!;
                var addCall = Expression.Call(property, addMethod, rParam);

                return addCall;
            })
            .ToList();

        // Create block expression
        // The list of expressions is passed to Expression.Block as a parameter.
        // Expression.Block executes the expressions in order.
        // It returns the last expression in the block, which is rParam (the TEntity).
        var block = Expression.Block([..methods, rParam]);

        // Dynamically constructs the corresponding Func type using reflection.
        List<Type> types = [typeof(TEntity), ..JoiningTables, typeof(TEntity)];
        var funcType = typeof(Func<>).Assembly
            .GetType($"System.Func`{types.Count}")! // Func type has an additional return type
            .MakeGenericType([..types]);

        // Create the lambda expression
        var lambda = Expression.Lambda(funcType, block, parameters);

        // Compile the expression tree into a delegate
        var map = lambda.Compile();
    }
}
