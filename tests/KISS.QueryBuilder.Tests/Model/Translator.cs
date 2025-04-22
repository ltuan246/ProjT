namespace KISS.QueryBuilder.Tests.Model;

public sealed record Translator : ExpressionTranslator
{
    // Property to store the translated SQL output
    public string TranslatedSql { get; private set; } = string.Empty;

    protected override void Visit(BinaryExpression binaryExpression)
    {
        base.Visit(binaryExpression);

        base.Visit(binaryExpression.Left);
        string left = TranslatedSql;
        base.Visit(binaryExpression.Right);
        string right = TranslatedSql;
        TranslatedSql = $"{left}{BinaryOperandMap[binaryExpression.NodeType]}{right}";
    }

    protected override void Visit(UnaryExpression unaryExpression)
    {
        base.Visit(unaryExpression);

        base.Visit(unaryExpression.Operand);
        string operand = TranslatedSql;
        TranslatedSql =
            $"{unaryExpression.NodeType switch { ExpressionType.Not => "NOT", _ => unaryExpression.NodeType.ToString() }} {operand}";
    }

    protected override void Visit(MemberExpression memberExpression)
    {
        base.Visit(memberExpression);
        TranslatedSql = memberExpression.Member.Name; // Simplified: just the property name
    }

    protected override void Visit(ConstantExpression constantExpression)
    {
        base.Visit(constantExpression);

        var val = constantExpression.Value switch
        {
            string s => $"'{s}'",
            null => "NULL",
            _ => constantExpression.Value.ToString()
        };

        TranslatedSql = val!;
    }

    protected override void Visit(NewExpression newExpression)
    {
        base.Visit(newExpression);
        TranslatedSql = $"NEW {newExpression.Type.Name}"; // Simplified representation
    }

    protected override void Visit(MemberInitExpression memberInitExpression)
    {
        base.Visit(memberInitExpression);

        var bindings = string.Join(", ", memberInitExpression.Bindings.Select(b =>
        {
            if (b is MemberAssignment assignment)
            {
                base.Visit(assignment.Expression);
                return $"{b.Member.Name} = {TranslatedSql}";
            }

            return b.Member.Name;
        }));

        TranslatedSql = $"NEW {memberInitExpression.Type.Name} {{ {bindings} }}";
    }

    protected override void Visit(MethodCallExpression methodCallExpression)
    {
        base.Visit(methodCallExpression);

        var args = string.Join(", ", methodCallExpression.Arguments.Select(arg =>
        {
            base.Visit(arg);
            return TranslatedSql;
        }));

        if (string.IsNullOrEmpty(args) && methodCallExpression.Object is not null)
        {
            base.Visit(methodCallExpression.Object);
            args = TranslatedSql;
        }

        TranslatedSql = $"{methodCallExpression.Method.Name}({args})";
    }

    protected override void Visit(LambdaExpression lambdaExpression)
    {
        base.Visit(lambdaExpression);

        base.Visit(lambdaExpression.Body);
        TranslatedSql = $"({string.Join(", ", lambdaExpression.Parameters.Select(p => p.Name))}) => {TranslatedSql}";
    }

    // Expose Translate for testing
    protected override void Visit(Expression? expression) => base.Visit(expression);
}
