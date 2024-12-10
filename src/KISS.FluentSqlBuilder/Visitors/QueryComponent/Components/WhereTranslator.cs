namespace KISS.FluentSqlBuilder.Visitors.QueryComponent.Components;

/// <summary>
///     A builder for a <c>WHERE</c> clause.
/// </summary>
/// <param name="Composite">The structure of the fluent SQL builder.</param>
public sealed record WhereTranslator(CompositeQuery Composite) : ExpressionTranslator
{
    private Dictionary<ExpressionType, string> BinaryOperandMap { get; } = new()
    {
        { ExpressionType.Assign, " = " },
        { ExpressionType.Equal, " = " },
        { ExpressionType.NotEqual, " != " },
        { ExpressionType.GreaterThan, " > " },
        { ExpressionType.GreaterThanOrEqual, " >= " },
        { ExpressionType.LessThan, " < " },
        { ExpressionType.LessThanOrEqual, " <= " },
        { ExpressionType.OrElse, " OR " },
        { ExpressionType.AndAlso, " AND " },
        { ExpressionType.Coalesce, " ?? " },
        { ExpressionType.Add, " + " },
        { ExpressionType.Subtract, " - " },
        { ExpressionType.Multiply, " * " },
        { ExpressionType.Divide, " / " },
        { ExpressionType.Modulo, " % " },
        { ExpressionType.And, " & " },
        { ExpressionType.Or, " | " },
        { ExpressionType.ExclusiveOr, " ^ " },
        { ExpressionType.LeftShift, " << " },
        { ExpressionType.RightShift, " >> " }
    };

    /// <inheritdoc />
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
                            Composite.AppendFormat($"{fieldInfo.GetValue(fieldType)}");
                            break;
                        // Accessing a static property, get its type and value using reflection
                        case PropertyInfo propertyInfo:
                            var propType = propertyInfo.GetType();
                            Composite.AppendFormat($"{propertyInfo.GetValue(propType)}");
                            break;
                    }

                    break;
                }

            // Accessing a property or field of a parameter in a lambda
            case ParameterExpression parameterExpression:
                {
                    Composite.Append(
                        $"{Composite.GetAliasMapping(parameterExpression.Type)}.{memberExpression.Member.Name}");
                    break;
                }

            // Accessing a field/property of a constant object
            case ConstantExpression constantExpression:
                {
                    var (evaluated, value) = Composite.GetValue(memberExpression);
                    if (evaluated)
                    {
                        Composite.AppendFormat(value);
                    }
                    else
                    {
                        Translate(constantExpression);
                    }

                    break;
                }

            // Accessing an object creation, method invocation, or method call (e.g., new Object().Property, obj.Method().Property)
            case NewExpression:
            case InvocationExpression:
            case MethodCallExpression:
                {
                    // Accessing the property or field (memberExpression.Member.Name) on the object or result of the method (memberExpression.Expression).
                    var member = Expression.Property(memberExpression.Expression, memberExpression.Member.Name);
                    var value = Expression.Lambda(member).Compile().DynamicInvoke();
                    Composite.AppendFormat($"{value}");
                    break;
                }

            default:
                {
                    Translate(memberExpression.Expression);
                    break;
                }
        }
    }

    /// <inheritdoc />
    protected override void Translate(ConstantExpression constantExpression)
        => Composite.AppendFormat($"{constantExpression.Value}");

    /// <inheritdoc />
    protected override void Translate(BinaryExpression binaryExpression)
    {
        if (binaryExpression.NodeType is ExpressionType.ArrayIndex)
        {
            // Handles array indexing in expressions, e.g., array[index]
            var arrayExpression = binaryExpression.Left;
            var indexExpression = binaryExpression.Right;
            var arrayAccessExpression = Expression.ArrayAccess(arrayExpression, indexExpression);
            var value = Expression.Lambda(arrayAccessExpression).Compile().DynamicInvoke();
            Composite.AppendFormat($"{value}");
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
                        Composite.OpenParentheses();
                        break;
                    }
            }

            Translate(binaryExpression.Left);
            Composite.Append(BinaryOperandMap[binaryExpression.NodeType]);
            Translate(binaryExpression.Right);
            Composite.CloseParentheses();
        }
    }

    /// <inheritdoc />
    protected override void Translate(MethodCallExpression methodCallExpression)
    {
        const string inRange = nameof(SqlFunctions.InRange);
        const string anyIn = nameof(SqlFunctions.AnyIn);
        const string notIn = nameof(SqlFunctions.NotIn);

        var mi = typeof(SqlFunctions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Single(mt => mt.IsGenericMethod && mt.Name == methodCallExpression.Method.Name);

        switch (mi.Name)
        {
            case inRange:
                {
                    const string betweenOp = " BETWEEN ";
                    const string andOp = " AND ";

                    if (methodCallExpression.Arguments is
                        [var fieldAsExpression, var beginAsExpression, var endAsExpression])
                    {
                        Composite.OpenParentheses();
                        Translate(fieldAsExpression);

                        Composite.Append(betweenOp);
                        Translate(beginAsExpression);

                        Composite.Append(andOp);
                        Translate(endAsExpression);
                        Composite.CloseParentheses();
                    }

                    break;
                }

            case anyIn:
                {
                    const string inOp = " IN ";

                    if (methodCallExpression.Arguments is
                        [var fieldAsExpression, var valuesAsExpression])
                    {
                        Composite.OpenParentheses();
                        Translate(fieldAsExpression);

                        Composite.Append(inOp);
                        Translate(valuesAsExpression);
                        Composite.CloseParentheses();
                    }

                    break;
                }

            case notIn:
                {
                    const string notInOp = " NOT IN ";

                    if (methodCallExpression.Arguments is
                        [var fieldAsExpression, var valuesAsExpression])
                    {
                        Composite.OpenParentheses();
                        Translate(fieldAsExpression);

                        Composite.Append(notInOp);
                        Translate(valuesAsExpression);
                        Composite.CloseParentheses();
                    }

                    break;
                }
        }
    }
}
