namespace KISS.FluentQueryBuilder.Builders;

public sealed partial class FluentBuilder : IQueryExpressionVisitor
{
    private readonly Dictionary<ExpressionType, string> _binaryOperandMap = new()
    {
        { ExpressionType.Assign, " = " },
        { ExpressionType.Equal, " == " },
        { ExpressionType.NotEqual, " != " },
        { ExpressionType.GreaterThan, " > " },
        { ExpressionType.GreaterThanOrEqual, " >= " },
        { ExpressionType.LessThan, " < " },
        { ExpressionType.LessThanOrEqual, " <= " },
        { ExpressionType.OrElse, " || " },
        { ExpressionType.AndAlso, " && " },
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

    public void Visit(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression binaryExpression:
                Visit(binaryExpression);
                break;

            case MemberExpression memberExpression:
                Visit(memberExpression);
                break;

            case ConstantExpression constantExpression:
                Visit(constantExpression);
                break;

            default:
                break;
        }
    }

    public void Visit([NotNull] BinaryExpression binaryExpression)
    {
        Visit(binaryExpression.Left);
        Append(_binaryOperandMap[binaryExpression.NodeType]);
        Visit(binaryExpression.Right);
    }

    public void Visit([NotNull] MemberExpression memberExpression)
    {
        PushState(memberExpression);

        Append(memberExpression.Member.Name);
        if (memberExpression.Expression is not null)
        {
            Visit(memberExpression.Expression);
        }

        PopState();
    }

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
            }
        }
    }
}
