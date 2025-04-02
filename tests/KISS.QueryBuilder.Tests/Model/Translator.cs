namespace KISS.QueryBuilder.Tests.Model;

public sealed record Translator : ExpressionTranslator
{
    // Property to store the translated SQL output
    public string TranslatedSql { get; private set; } = string.Empty;

    protected override void Translate(BinaryExpression binaryExpression)
    {
        base.Translate(binaryExpression);

        Translate(binaryExpression.Left);
        string left = TranslatedSql;
        Translate(binaryExpression.Right);
        string right = TranslatedSql;
        TranslatedSql = $"{left}{BinaryOperandMap[binaryExpression.NodeType]}{right}";
    }

    protected override void Translate(UnaryExpression unaryExpression)
    {
        base.Translate(unaryExpression);

        Translate(unaryExpression.Operand);
        string operand = TranslatedSql;
        TranslatedSql =
            $"{unaryExpression.NodeType switch { ExpressionType.Not => "NOT", _ => unaryExpression.NodeType.ToString() }} {operand}";
    }

    protected override void Translate(MemberExpression memberExpression)
    {
        base.Translate(memberExpression);
        TranslatedSql = memberExpression.Member.Name; // Simplified: just the property name
    }

    protected override void Translate(ConstantExpression constantExpression)
    {
        base.Translate(constantExpression);

        var val = constantExpression.Value switch
        {
            string s => $"'{s}'",
            null => "NULL",
            _ => constantExpression.Value.ToString()
        };

        TranslatedSql = val!;
    }

    protected override void Translate(NewExpression newExpression)
    {
        base.Translate(newExpression);
        TranslatedSql = $"NEW {newExpression.Type.Name}"; // Simplified representation
    }

    protected override void Translate(MemberInitExpression memberInitExpression)
    {
        base.Translate(memberInitExpression);

        var bindings = string.Join(", ", memberInitExpression.Bindings.Select(b =>
        {
            if (b is MemberAssignment assignment)
            {
                Translate(assignment.Expression);
                return $"{b.Member.Name} = {TranslatedSql}";
            }

            return b.Member.Name;
        }));

        TranslatedSql = $"NEW {memberInitExpression.Type.Name} {{ {bindings} }}";
    }

    protected override void Translate(MethodCallExpression methodCallExpression)
    {
        base.Translate(methodCallExpression);

        var args = string.Join(", ", methodCallExpression.Arguments.Select(arg =>
        {
            Translate(arg);
            return TranslatedSql;
        }));

        if (string.IsNullOrEmpty(args) && methodCallExpression.Object is not null)
        {
            Translate(methodCallExpression.Object);
            args = TranslatedSql;
        }

        TranslatedSql = $"{methodCallExpression.Method.Name}({args})";
    }

    protected override void Translate(LambdaExpression lambdaExpression)
    {
        base.Translate(lambdaExpression);

        Translate(lambdaExpression.Body);
        TranslatedSql = $"({string.Join(", ", lambdaExpression.Parameters.Select(p => p.Name))}) => {TranslatedSql}";
    }

    // Expose Translate for testing
    public void Visit(Expression expression) => Translate(expression);
}
