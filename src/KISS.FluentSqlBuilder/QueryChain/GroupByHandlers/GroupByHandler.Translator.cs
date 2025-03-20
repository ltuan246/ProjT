namespace KISS.FluentSqlBuilder.QueryChain.GroupByHandlers;

/// <summary>
///     A handler for processing GROUP BY clauses in a query chain.
///     This class provides the translation logic for converting grouping expressions
///     into SQL-compatible form.
/// </summary>
public sealed partial record GroupByHandler
{
    /// <summary>
    ///     Translates a member expression into SQL for GROUP BY clauses.
    ///     Handles property and field access for grouping.
    /// </summary>
    /// <param name="memberExpression">The member expression to translate.</param>
    /// <exception cref="NotSupportedException">Thrown when the expression is not supported.</exception>
    protected override void Translate(MemberExpression memberExpression)
    {
        if (memberExpression is { Expression: ParameterExpression parameterExpression })
        {
            string alias = Composite.GetAliasMapping(parameterExpression.Type);
            string fieldName = $"{alias}_{memberExpression.Member.Name}";
            Composite.GroupingKeys[fieldName] = memberExpression.Type;
            Append($"{fieldName}");
        }
        else
        {
            throw new NotSupportedException("Expression not supported.");
        }
    }

    /// <summary>
    ///     Translates a unary expression into SQL for GROUP BY clauses.
    ///     Handles operations like type conversion and negation.
    /// </summary>
    /// <param name="unaryExpression">The unary expression to translate.</param>
    /// <exception cref="NotSupportedException">Thrown when the expression is not supported.</exception>
    protected override void Translate(UnaryExpression unaryExpression)
    {
        if (unaryExpression is { Operand: MemberExpression memberExpression })
        {
            string alias = Composite.GetAliasMapping(memberExpression.Member.DeclaringType!);
            string fieldName = $"{alias}_{memberExpression.Member.Name}";
            Composite.GroupingKeys[fieldName] = memberExpression.Type;
            Append($"{fieldName}");
        }
        else
        {
            throw new NotSupportedException("Expression not supported.");
        }
    }
}
