namespace KISS.FluentSqlBuilder.QueryChain.OrderByHandlers;

/// <summary>
///     A handler for processing ORDER BY clauses in a query chain.
///     This class provides the translation logic for converting sorting expressions
///     into SQL-compatible form.
/// </summary>
public sealed partial record OrderByHandler
{
    /// <summary>
    ///     Translates a member expression into SQL for ORDER BY clauses.
    ///     Handles property and field access for sorting.
    /// </summary>
    /// <param name="memberExpression">The member expression to translate.</param>
    /// <exception cref="NotSupportedException">Thrown when the expression is not supported.</exception>
    protected override void Visit(MemberExpression memberExpression)
    {
        if (memberExpression is { Expression: ParameterExpression parameterExpression })
        {
            Append($"{Composite.GetAliasMapping(parameterExpression.Type)}.{memberExpression.Member.Name}");
        }
        else
        {
            throw new NotSupportedException("Expression not supported.");
        }
    }
}
