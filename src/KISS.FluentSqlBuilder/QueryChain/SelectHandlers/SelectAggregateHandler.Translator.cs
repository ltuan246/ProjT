namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     Provides translation logic for aggregate SELECT clauses in a query chain.
///     This class converts aggregate expressions into SQL-compatible syntax for use in
///     SELECT statements with aggregate functions.
/// </summary>
public sealed partial record SelectAggregateHandler
{
    /// <summary>
    ///     Translates a member expression into SQL for aggregate operations.
    ///     Handles property and field access within aggregate functions, and registers
    ///     the aggregation key and its type for later use in the query.
    /// </summary>
    /// <param name="memberExpression">The member expression to translate.</param>
    protected override void Visit(MemberExpression memberExpression)
    {
        if (memberExpression is { Expression: ParameterExpression parameterExpression })
        {
            Append($"{Composite.GetAliasMapping(parameterExpression.Type)}_{memberExpression.Member.Name}");
            switch (memberExpression.Member)
            {
                // If accessing a static property, register its type for aggregation.
                case PropertyInfo propertyInfo:
                    var propType = propertyInfo.PropertyType;
                    ((GroupByDecorator)Composite).AggregationKeys[Alias] = propType;
                    break;
            }
        }
    }

    /// <summary>
    ///     Translates a unary expression into SQL for aggregate operations.
    ///     Handles operations like negation and type conversion by recursively visiting the operand.
    /// </summary>
    /// <param name="unaryExpression">The unary expression to translate.</param>
    protected override void Visit(UnaryExpression unaryExpression)
    {
        if (unaryExpression is { Operand: { } expression })
        {
            Visit(expression);
        }
    }

    /// <summary>
    ///     Translates a lambda expression into SQL for aggregate operations.
    ///     Processes the body of lambda expressions used in aggregates.
    /// </summary>
    /// <param name="lambdaExpression">The lambda expression to translate.</param>
    protected override void Visit(LambdaExpression lambdaExpression)
    {
        if (lambdaExpression is { Body: { } expression })
        {
            Visit(expression);
        }
    }

    /// <summary>
    ///     Translates a method call expression into SQL for aggregate operations.
    ///     Handles SQL function calls and custom aggregate methods, appending the result alias.
    /// </summary>
    /// <param name="methodCallExpression">The method call expression to translate.</param>
    protected override void Visit(MethodCallExpression methodCallExpression)
    {
        if (methodCallExpression is { Arguments: [{ } expression] })
        {
            Append($"{methodCallExpression.Method.Name}(");
            Visit(expression);
            Append($") AS {Alias} ");
        }
    }
}
