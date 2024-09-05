namespace KISS.FluentQueryBuilder.Builders;

public sealed partial record FluentBuilder<TEntity>
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

            case ParameterExpression parameterExpression:
                Translate(parameterExpression);
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
            case ExpressionType.Or:
            case ExpressionType.OrElse:
            case ExpressionType.And:
            case ExpressionType.AndAlso:
                OpenParentheses();
                Translate(binaryExpression.Left);
                Append(BinaryOperandMap[binaryExpression.NodeType]);
                Translate(binaryExpression.Right);
                CloseParentheses();
                break;

            case ExpressionType.ArrayIndex:
                var leftExpression = (MemberExpression)Visit(binaryExpression.Left);
                var constantExpression = (ConstantExpression)leftExpression.Expression!;
                var declaringType = constantExpression.Type;
                var declaringObject = constantExpression.Value;

                const MemberTypes memberTypes = MemberTypes.Field
                                                | MemberTypes.Property;
                const BindingFlags bindingFlags = BindingFlags.Public
                                                  | BindingFlags.NonPublic
                                                  | BindingFlags.Instance
                                                  | BindingFlags.Static;

                var member = declaringType
                    .GetMember(leftExpression.Member.Name, memberTypes, bindingFlags)
                    .Single();

                var arr = (Array)((FieldInfo)member).GetValue(declaringObject)!;

                var rightExpression = (ConstantExpression)Visit(binaryExpression.Right);
                var idx = (int)rightExpression.Value!;

                AppendFormat($"{arr.GetValue(idx)}");
                break;

            default:
                Translate(binaryExpression.Left);
                Append(BinaryOperandMap[binaryExpression.NodeType]);
                Translate(binaryExpression.Right);
                break;
        }
    }

    /// <summary>
    ///     Visits the children of the MemberExpression.
    /// </summary>
    /// <param name="memberExpression">The nodes to visit.</param>
    private void Translate(MemberExpression memberExpression)
    {
        PushState(memberExpression);

        if (memberExpression.Expression is not null)
        {
            Translate(memberExpression.Expression);
        }
        else
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
        }

        PopState();
    }

    /// <summary>
    ///     Visits the children of the ConstantExpression.
    /// </summary>
    /// <param name="constantExpression">The nodes to visit.</param>
    private void Translate(ConstantExpression constantExpression)
    {
        if (State.TryPeek(out var currentState))
        {
            switch (currentState.Expression)
            {
                case MemberExpression memberExpression:
                    var declaringType = constantExpression.Type;
                    var declaringObject = constantExpression.Value;

                    const MemberTypes memberTypes = MemberTypes.Field
                                                    | MemberTypes.Property;
                    const BindingFlags bindingFlags = BindingFlags.Public
                                                      | BindingFlags.NonPublic
                                                      | BindingFlags.Instance
                                                      | BindingFlags.Static;

                    var member = declaringType
                        .GetMember(memberExpression.Member.Name, memberTypes, bindingFlags)
                        .Single();

                    AppendFormat($"{((FieldInfo)member).GetValue(declaringObject)}");
                    break;
            }
        }
        else
        {
            AppendFormat($"{constantExpression.Value}");
        }
    }

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
                    const string betweenOpr = " BETWEEN ";
                    const string andOpr = " AND ";

                    if (methodCallExpression.Arguments is
                        [var fieldAsExpression, var beginAsExpression, var endAsExpression])
                    {
                        OpenParentheses();
                        Translate(fieldAsExpression);

                        Append(betweenOpr);
                        Translate(beginAsExpression);

                        Append(andOpr);
                        Translate(endAsExpression);
                        CloseParentheses();
                    }

                    break;
                }

            case anyIn:
                {
                    const string inOpr = " IN ";

                    if (methodCallExpression.Arguments is
                        [var fieldAsExpression, var valuesAsExpression])
                    {
                        OpenParentheses();
                        Translate(fieldAsExpression);

                        Append(inOpr);
                        Translate(valuesAsExpression);
                        CloseParentheses();
                    }

                    break;
                }

            case notIn:
                {
                    const string notInOpr = " NOT IN ";

                    if (methodCallExpression.Arguments is
                        [var fieldAsExpression, var valuesAsExpression])
                    {
                        OpenParentheses();
                        Translate(fieldAsExpression);

                        Append(notInOpr);
                        Translate(valuesAsExpression);
                        CloseParentheses();
                    }

                    break;
                }
        }
    }

    /// <summary>
    ///     Visits the children of the ParameterExpression.
    /// </summary>
    /// <param name="parameterExpression">The nodes to visit.</param>
    private void Translate(ParameterExpression parameterExpression)
    {
        _ = parameterExpression;
        if (State.TryPeek(out var currentState))
        {
            switch (currentState.Expression)
            {
                case MemberExpression memberExpression:
                    Append($"{memberExpression.Member.Name}");
                    break;
            }
        }
    }
}
