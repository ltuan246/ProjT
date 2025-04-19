namespace KISS.FluentSqlBuilder.QueryChain.WhereHandlers;

/// <summary>
///     A handler for processing WHERE clauses in a query chain.
///     This class provides the translation logic for converting LINQ expressions into SQL WHERE conditions.
/// </summary>
public sealed partial record WhereHandler
{
    /// <summary>
    ///     Translates a member expression into SQL.
    ///     Handles various scenarios for accessing members of objects, including static members,
    ///     parameter members, constant members, and method call results.
    /// </summary>
    /// <param name="memberExpression">The member expression to translate.</param>
    protected override void Translate(MemberExpression memberExpression)
    {
        switch (memberExpression.Expression)
        {
            // Accessing a static member (constant or static field)
            case null:
                {
                    switch (memberExpression.Member)
                    {
                        // Accessing a static field, get its type and value using reflection
                        case FieldInfo fieldInfo:
                            var fieldType = fieldInfo.GetType();
                            AppendFormat($"{fieldInfo.GetValue(fieldType)}");
                            break;
                        // Accessing a static property, get its type and value using reflection
                        case PropertyInfo propertyInfo:
                            var propType = propertyInfo.GetType();
                            AppendFormat($"{propertyInfo.GetValue(propType)}");
                            break;
                    }

                    break;
                }

            // Accessing a property or field of a parameter in a lambda
            case ParameterExpression parameterExpression:
                {
                    Append($"{Composite.GetAliasMapping(parameterExpression.Type)}.{memberExpression.Member.Name}");
                    break;
                }

            default:
                {
                    var (evaluated, value) = Composite.GetValue(memberExpression);
                    if (evaluated)
                    {
                        AppendFormat(value);
                    }
                    else
                    {
                        Translate(memberExpression.Expression);
                    }

                    break;
                }
        }
    }

    /// <summary>
    ///     Translates a constant expression into SQL.
    ///     Converts constant values into their string representation for SQL.
    /// </summary>
    /// <param name="constantExpression">The constant expression to translate.</param>
    protected override void Translate(ConstantExpression constantExpression)
        => AppendFormat($"{constantExpression.Value}");

    /// <summary>
    ///     Translates a binary expression into SQL.
    ///     Handles logical operations, comparisons, and array indexing.
    /// </summary>
    /// <param name="binaryExpression">The binary expression to translate.</param>
    protected override void Translate(BinaryExpression binaryExpression)
    {
        if (binaryExpression.NodeType is ExpressionType.ArrayIndex)
        {
            // Handles array indexing in expressions, e.g., array[index]
            var arrayExpression = binaryExpression.Left;
            var indexExpression = binaryExpression.Right;
            var arrayAccessExpression = Expression.ArrayAccess(arrayExpression, indexExpression);
            var value = Expression.Lambda(arrayAccessExpression).Compile().DynamicInvoke();
            AppendFormat($"{value}");
        }
        else
        {
            // Adds parentheses around logical operations (AND, OR)
            switch (binaryExpression.NodeType)
            {
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    {
                        OpenParentheses();
                        break;
                    }
            }

            Translate(binaryExpression.Left);
            Append(BinaryOperandMap[binaryExpression.NodeType]);
            Translate(binaryExpression.Right);
            CloseParentheses();
        }
    }

    /// <summary>
    ///     Translates a unary expression into SQL for aggregate operations.
    ///     Handles operations like negation and type conversion.
    /// </summary>
    /// <param name="unaryExpression">The unary expression to translate.</param>
    protected override void Translate(UnaryExpression unaryExpression)
    {
        if (unaryExpression is { Operand: { } expression })
        {
            Translate(expression);
        }
    }

    /// <summary>
    ///     Translates a method call expression into SQL.
    ///     Handles special SQL functions like InRange, AnyIn, and NotIn.
    /// </summary>
    /// <param name="methodCallExpression">The method call expression to translate.</param>
    protected override void Translate(MethodCallExpression methodCallExpression)
    {
        switch (methodCallExpression)
        {
            case { Method: { } t } when t.DeclaringType == typeof(SqlFunctions):
                const string inRange = nameof(SqlFunctions.InRange);
                const string anyIn = nameof(SqlFunctions.AnyIn);
                const string notIn = nameof(SqlFunctions.NotIn);

                switch (t.Name)
                {
                    case inRange:
                        {
                            const string betweenOp = " BETWEEN ";
                            const string andOp = " AND ";

                            if (methodCallExpression.Arguments is
                                [var fieldAsExpression, var beginAsExpression, var endAsExpression])
                            {
                                OpenParentheses();
                                Translate(fieldAsExpression);

                                Append(betweenOp);
                                Translate(beginAsExpression);

                                Append(andOp);
                                Translate(endAsExpression);
                                CloseParentheses();
                            }

                            break;
                        }

                    case anyIn:
                        {
                            const string inOp = " IN ";

                            if (methodCallExpression.Arguments is
                                [var fieldAsExpression, var valuesAsExpression])
                            {
                                OpenParentheses();
                                Translate(fieldAsExpression);

                                Append(inOp);
                                Translate(valuesAsExpression);
                                CloseParentheses();
                            }

                            break;
                        }

                    case notIn:
                        {
                            const string notInOp = " NOT IN ";

                            if (methodCallExpression.Arguments is
                                [var fieldAsExpression, var valuesAsExpression])
                            {
                                OpenParentheses();
                                Translate(fieldAsExpression);

                                Append(notInOp);
                                Translate(valuesAsExpression);
                                CloseParentheses();
                            }

                            break;
                        }
                }

                break;

            default:
                var (evaluated, value) = Composite.GetValue(methodCallExpression);
                break;
        }
    }
}
