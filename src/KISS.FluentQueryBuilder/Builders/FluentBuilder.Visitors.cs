namespace KISS.FluentQueryBuilder.Builders;

public sealed partial record FluentBuilder<TEntity> : IQueryExpressionVisitor
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

    /// <inheritdoc />
    public void Visit(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression binaryExpression:
                VisitBinary(binaryExpression);
                break;

            case MemberExpression memberExpression:
                Visit(memberExpression);
                break;

            case ConstantExpression constantExpression:
                Visit(constantExpression);
                break;

            case NewExpression newExpression:
                Visit(newExpression);
                break;

            case MethodCallExpression methodCallExpression:
                Visit(methodCallExpression);
                break;
        }
    }

    /// <inheritdoc />
    public void Visit([NotNull] BinaryExpression binaryExpression)
    {
        Visit(binaryExpression.Left);
        Append(BinaryOperandMap[binaryExpression.NodeType]);
        Visit(binaryExpression.Right);
    }

    /// <inheritdoc />
    public void Visit([NotNull] MemberExpression memberExpression)
    {
        PushState(memberExpression);

        if (memberExpression.Expression is not null)
        {
            if (memberExpression.Expression is ParameterExpression)
            {
                Append($"{memberExpression.Member.Name}");
            }

            Visit(memberExpression.Expression);
        }
        else
        {
            if (memberExpression.Member is PropertyInfo propertyInfo)
            {
                var propType = propertyInfo.GetType();
                AppendFormat($"{propertyInfo.GetValue(propType)}");
            }
        }

        PopState();
    }

    /// <inheritdoc />
    public void Visit([NotNull] ConstantExpression constantExpression)
    {
        if (State.TryPeek(out var currentState))
        {
            if (currentState.Expression is MemberExpression memberExpression)
            {
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
                return;
            }
        }

        AppendFormat($"{constantExpression.Value}");
    }

    /// <inheritdoc />
    public void Visit([NotNull] NewExpression newExpression)
    {
        const string comma = ", ";
        using var enumerator = newExpression.Arguments.GetEnumerator();
        if (enumerator.MoveNext())
        {
            Visit(enumerator.Current);
            while (enumerator.MoveNext())
            {
                Append(comma);
                Visit(enumerator.Current);
            }
        }
    }

    /// <inheritdoc />
    public void Visit([NotNull] MethodCallExpression methodCallExpression)
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
                        Visit(fieldAsExpression);

                        Append(betweenOpr);
                        Visit(beginAsExpression);

                        Append(andOpr);
                        Visit(endAsExpression);
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
                        Visit(fieldAsExpression);

                        Append(inOpr);
                        Visit(valuesAsExpression);
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
                        Visit(fieldAsExpression);

                        Append(notInOpr);
                        Visit(valuesAsExpression);
                        CloseParentheses();
                    }

                    break;
                }

            default:
                break;
        }
    }

    /// <summary>
    ///     Visits the children of the BinaryExpression.
    /// </summary>
    /// <param name="binaryExpression">The nodes to visit.</param>
    private void VisitBinary([NotNull] BinaryExpression binaryExpression)
    {
        switch (binaryExpression.NodeType)
        {
            case ExpressionType.AndAlso:
            case ExpressionType.OrElse:
                OpenParentheses();
                Visit(binaryExpression);
                CloseParentheses();
                break;

            default:
                Visit(binaryExpression);
                break;
        }
    }
}
