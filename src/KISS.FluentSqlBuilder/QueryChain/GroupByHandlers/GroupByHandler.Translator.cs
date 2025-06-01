namespace KISS.FluentSqlBuilder.QueryChain.GroupByHandlers;

/// <summary>
///     Provides translation logic for processing GROUP BY clauses in a query chain.
///     This class converts grouping expressions into SQL-compatible syntax for use in
///     GROUP BY statements.
/// </summary>
public sealed partial record GroupByHandler
{
    /// <summary>
    ///     Translates a member expression into its SQL representation for GROUP BY clauses.
    ///     Handles property and field access by generating the appropriate alias and field name.
    /// </summary>
    /// <param name="memberExpression">The member expression to translate.</param>
    /// <exception cref="NotSupportedException">Thrown when the expression is not supported for translation.</exception>
    protected override void Visit(MemberExpression memberExpression)
    {
        if (memberExpression is { Expression: ParameterExpression parameterExpression })
        {
            var alias = Composite.GetAliasMapping(parameterExpression.Type);
            var fieldName = $"{alias}_{memberExpression.Member.Name}";
            ((GroupByDecorator)Composite).GroupingKeys[fieldName] = memberExpression.Type;
            Append($"{fieldName}");
        }
        else
        {
            throw new NotSupportedException("Expression not supported.");
        }
    }
}
