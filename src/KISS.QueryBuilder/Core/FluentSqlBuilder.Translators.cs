namespace KISS.QueryBuilder.Core;

/// <summary>
///     Declares operations for the <see cref="FluentSqlBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of results to return.</typeparam>
public sealed partial class FluentSqlBuilder<TEntity>
{
    private Dictionary<ExpressionType, string> BinaryOperandMap { get; } = new()
    {
        { ExpressionType.Assign, " = " },
        { ExpressionType.Equal, " == " },
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
        switch (binaryExpression.NodeType)
        {
            case ExpressionType.ArrayIndex:
                {
                    var (evaluated, value) = GetValue(binaryExpression);
                    if (evaluated)
                    {
                        AppendFormat(value);
                    }

                    return;
                }

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

    /// <summary>
    ///     Visits the children of the MemberExpression.
    /// </summary>
    /// <param name="memberExpression">The nodes to visit.</param>
    private void Translate(MemberExpression memberExpression)
    {
        switch (memberExpression.Expression)
        {
            case null:
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

            case ParameterExpression:
                string tableAlias = GetTableAlias(memberExpression.Member.DeclaringType!);
                Append($"{tableAlias}.{memberExpression.Member.Name}");
                break;

            case ConstantExpression constantExpression:
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

            default:
                Translate(memberExpression.Expression);
                break;
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

        Append(string.Join(Comma, selectList));
    }

    /// <summary>
    ///     Visits the children of the MethodCallExpression.
    /// </summary>
    /// <param name="methodCallExpression">The nodes to visit.</param>
    private void Translate(MethodCallExpression methodCallExpression)
    {
        const string inRange = nameof(SqlExpression.InRange);
        const string anyIn = nameof(SqlExpression.AnyIn);
        const string notIn = nameof(SqlExpression.NotIn);

        var mi = typeof(SqlExpression)
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
