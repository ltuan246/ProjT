namespace KISS.FluentSqlBuilder.Visitor;

/// <summary>
///     A custom expression visitor that translates LINQ expressions into SQL-compatible syntax.
///     This class provides the foundation for converting C# expressions into SQL statements
///     by implementing a visitor pattern for different types of expressions.
/// </summary>
public abstract record ExpressionTranslator : SimpleExpressionVisitor
{
    /// <summary>
    ///     Maps C# expression types to their corresponding SQL operators and symbols.
    ///     This dictionary provides the translation rules for converting C# expressions
    ///     into SQL-compatible syntax.
    /// </summary>
    protected Dictionary<ExpressionType, string> BinaryOperandMap { get; } = new()
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
}
