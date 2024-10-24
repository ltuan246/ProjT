namespace KISS.QueryBuilder.Core;

/// <summary>
///     Declares operations for the <see cref="FluentSqlBuilder{TRecordset}" /> type.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public sealed partial class FluentSqlBuilder<TRecordset>
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

    /// <summary>
    ///     Dispatches the expression to one of the more specialized visit methods in this class.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    private void Translate(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression binaryExpression:
                Translate(binaryExpression);
                break;

            case MemberExpression memberExpression:
                Translate(memberExpression);
                break;

            case ConstantExpression constantExpression:
                Translate(constantExpression);
                break;

            case NewExpression newExpression:
                Translate(newExpression);
                break;

            case MethodCallExpression methodCallExpression:
                Translate(methodCallExpression);
                break;
        }
    }

    /// <summary>
    ///     Visits the children of the BinaryExpression.
    /// </summary>
    /// <param name="binaryExpression">The nodes to visit.</param>
    private void Translate(BinaryExpression binaryExpression)
    {
        if (binaryExpression.NodeType is ExpressionType.ArrayIndex)
        {
            var arrayExpression = binaryExpression.Left;
            var indexExpression = binaryExpression.Right;
            var arrayAccessExpression = Expression.ArrayAccess(arrayExpression, indexExpression);
            var value = Expression.Lambda(arrayAccessExpression).Compile().DynamicInvoke();
            AppendFormat($"{value}");
        }
        else
        {
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
    ///     Visits the children of the MemberExpression.
    /// </summary>
    /// <param name="memberExpression">The nodes to visit.</param>
    private void Translate(MemberExpression memberExpression)
    {
        switch (memberExpression.Expression)
        {
            // Accessing a static member (constant or static field)
            case null:
                {
                    switch (memberExpression.Member)
                    {
                        case FieldInfo fieldInfo:
                            var fieldType = fieldInfo.GetType();
                            AppendFormat($"{fieldInfo.GetValue(fieldType)}");
                            break;
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
                    Append($"{GetTableAlias(parameterExpression.Type)}.{memberExpression.Member.Name}");
                    break;
                }

            // Accessing a field/property of a constant object
            case ConstantExpression constantExpression:
                {
                    var (evaluated, value) = GetValue(memberExpression);
                    if (evaluated)
                    {
                        AppendFormat(value);
                    }
                    else
                    {
                        Translate(constantExpression);
                    }

                    break;
                }

            case NewExpression:
            case InvocationExpression:
            case MethodCallExpression:
                {
                    var member = Expression.Property(memberExpression.Expression, memberExpression.Member.Name);
                    var value = Expression.Lambda(member).Compile().DynamicInvoke();
                    AppendFormat($"{value}");
                    break;
                }

            default:
                {
                    /*
                     Possible Expression Types in MemberExpression.Expression:

                     1. ParameterExpression:
                        - Represents a parameter in a lambda expression.
                        - Example: p => p.Age (p is a ParameterExpression)

                     2. ConstantExpression:
                        - Represents a constant value or a static member.
                        - Example: () => Person.StaticAge (Person is a ConstantExpression)

                     3. NewExpression:
                        - Represents the creation of a new object.
                        - Example: new Person().Age (new Person() is a NewExpression)

                     4. MemberExpression:
                        - Represents another member access, allowing for chaining.
                        - Example: person.Address.Street (person.Address is a MemberExpression)

                     5. InvocationExpression:
                        - Represents a method invocation.
                        - Example: someObject.Method().Property (someObject.Method() is an InvocationExpression)

                     6. MethodCallExpression:
                        - Represents a method call that returns a member.
                        - Example: GetPerson().Age (GetPerson() is a MethodCallExpression)

                     7. UnaryExpression:
                        - Represents a unary operation, such as negation or type conversion.
                        - Example: -(x.Age) (UnaryExpression representing the negation)

                     8. BinaryExpression:
                        - Represents a binary operation, like addition or comparison.
                        - Example: person.Age > 18 (BinaryExpression representing the comparison)
                     */
                    Translate(memberExpression.Expression);
                    break;
                }
        }
    }

    /// <summary>
    ///     Visits the children of the ConstantExpression.
    /// </summary>
    /// <param name="constantExpression">The nodes to visit.</param>
    private void Translate(ConstantExpression constantExpression)
        => AppendFormat($"{constantExpression.Value}");

    /// <summary>
    ///     Visits the children of the NewExpression.
    /// </summary>
    /// <param name="newExpression">The nodes to visit.</param>
    private void Translate(NewExpression newExpression)
    {
        var selectList = newExpression.Members!
            .Select(m => m.Name)
            .Zip(newExpression.Arguments, (name, arg) =>
            {
                if (arg is MemberExpression memberExpression)
                {
                    string tableAlias = GetTableAlias(memberExpression.Member.DeclaringType!);
                    return $"{tableAlias}." + (name == memberExpression.Member.Name
                        ? memberExpression.Member.Name
                        : $"{memberExpression.Member.Name} AS {name}");
                }

                return name;
            })
            .ToArray();

        Append(string.Join(ClauseConstants.Comma, selectList));
    }

    /// <summary>
    ///     Visits the children of the MethodCallExpression.
    /// </summary>
    /// <param name="methodCallExpression">The nodes to visit.</param>
    private void Translate(MethodCallExpression methodCallExpression)
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
    }
}
