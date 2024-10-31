namespace KISS.FluentQueryBuilder.Builders;

/// <summary>
///     Declares operations for the <see cref="FluentBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public sealed partial class FluentBuilder<TEntity>
{
    private Dictionary<ExpressionType, string> BinaryOperandMap { get; } = new()
    {
        { Assign, " = " },
        { Equal, " == " },
        { NotEqual, " != " },
        { GreaterThan, " > " },
        { GreaterThanOrEqual, " >= " },
        { LessThan, " < " },
        { LessThanOrEqual, " <= " },
        { OrElse, " OR " },
        { AndAlso, " AND " },
        { Coalesce, " ?? " },
        { Add, " + " },
        { Subtract, " - " },
        { Multiply, " * " },
        { Divide, " / " },
        { Modulo, " % " },
        { And, " & " },
        { Or, " | " },
        { ExclusiveOr, " ^ " },
        { LeftShift, " << " },
        { RightShift, " >> " }
    };

    // private List<(Type, string[] PropertyNames)> PreferredPropertyOrders { get; } =
    // [
    //     (typeof(LambdaExpression), ["Parameters", "Body"]),
    //     (typeof(BinaryExpression), ["Left", "Right", "Conversion"]),
    //     (typeof(BlockExpression), ["Variables", "Expressions"]),
    //     (typeof(CatchBlock), ["Variable", "Filter", "Body"]),
    //     (typeof(ConditionalExpression), ["Test", "IfTrue", "IfFalse"]),
    //     (typeof(IndexExpression), ["Object", "Arguments"]),
    //     (typeof(InvocationExpression), ["Arguments", "Expression"]),
    //     (typeof(ListInitExpression), ["NewExpression", "Initializers"]),
    //     (typeof(MemberInitExpression), ["NewExpression", "Bindings"]),
    //     (typeof(MethodCallExpression), ["Object", "Arguments"]),
    //     (typeof(SwitchCase), ["TestValues", "Body"]),
    //     (typeof(SwitchExpression), ["SwitchValue", "Cases", "DefaultBody"]),
    //     (typeof(TryExpression), ["Body", "Handlers", "Finally", "Fault"]),
    //     (typeof(DynamicExpression), ["Binder", "Arguments"])
    // ];

    // private HashSet<ExpressionType> RelationalOperators { get; } =
    // [
    //     Equal,
    //     GreaterThan,
    //     GreaterThanOrEqual,
    //     LessThan,
    //     LessThanOrEqual,
    //     NotEqual
    // ];

    // private HashSet<ExpressionType> BinaryExpressionTypes { get; } =
    // [
    //     Add,
    //     AddChecked,
    //     Divide,
    //     Modulo,
    //     Multiply,
    //     MultiplyChecked,
    //     Power,
    //     Subtract,
    //     SubtractChecked, // mathematical operators
    //     And,
    //     Or,
    //     ExclusiveOr, // bitwise / logical operations
    //     LeftShift,
    //     RightShift, // shift operators
    //     AndAlso,
    //     OrElse, // short-circuit boolean operators
    //     Equal,
    //     NotEqual,
    //     GreaterThanOrEqual,
    //     GreaterThan,
    //     LessThan,
    //     LessThanOrEqual, // comparison operators
    //     Coalesce,
    //     ArrayIndex,
    //     Assign,
    //     AddAssign,
    //     AddAssignChecked,
    //     DivideAssign,
    //     ModuloAssign,
    //     MultiplyAssign,
    //     MultiplyAssignChecked,
    //     PowerAssign,
    //     SubtractAssign,
    //     SubtractAssignChecked,
    //     AndAssign,
    //     OrAssign,
    //     ExclusiveOrAssign,
    //     LeftShiftAssign,
    //     RightShiftAssign
    // ];

    // private HashSet<ExpressionType> UnaryExpressionTypes { get; } =
    // [
    //     ArrayLength,
    //     ExpressionType.Convert,
    //     ConvertChecked,
    //     Unbox,
    //     Negate,
    //     NegateChecked,
    //     Not,
    //     Quote,
    //     TypeAs,
    //     UnaryPlus,
    //     IsTrue,
    //     IsFalse,
    //     PreIncrementAssign,
    //     PreDecrementAssign,
    //     PostIncrementAssign,
    //     PostDecrementAssign,
    //     Increment,
    //     Decrement,
    //     Throw
    // ];

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
            case ArrayIndex:
                {
                    var (evaluated, value) = GetValue(binaryExpression);
                    if (evaluated)
                    {
                        AppendFormat(value);
                    }

                    return;
                }

            case Or:
            case OrElse:
            case And:
            case AndAlso:
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
                Append(memberExpression.Member.Name);
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
        using var enumerator = newExpression.Arguments.GetEnumerator();
        if (enumerator.MoveNext())
        {
            Translate(enumerator.Current);
            while (enumerator.MoveNext())
            {
                AddCommaSeparated();
                Translate(enumerator.Current);
            }
        }
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
