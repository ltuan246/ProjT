namespace KISS.FluentSqlBuilder.QueryChain.JoinHandlers;

/// <summary>
///     A handler for processing join operations in a query chain, linking two relations via key equality.
///     Provides functionality to translate member expressions into SQL column references.
/// </summary>
/// <typeparam name="TRelation">
///     The type of the relation (source table or entity).
///     This type is used to generate proper table names and column mappings.
/// </typeparam>
/// <typeparam name="TReturn">
///     The combined type to return.
///     This type represents the result of the join operation.
/// </typeparam>
public abstract partial record JoinHandler<TRelation, TReturn>
{
    /// <summary>
    ///     Translates a binary expression into SQL.
    ///     Handles logical operations, comparisons, and array indexing.
    /// </summary>
    /// <param name="binaryExpression">The binary expression to translate.</param>
    protected override void Visit(BinaryExpression binaryExpression)
    {
        Append("INNER JOIN");
        AppendLine($"{TypeUtils.GetTableName(RelationType)} {Composite.GetAliasMapping(RelationType)}", true);
        AppendLine(" ON ", true);
        Visit(binaryExpression.Left);
        Append(BinaryOperandMap[binaryExpression.NodeType]);
        Visit(binaryExpression.Right);
    }

    /// <summary>
    ///     Translates a member expression into a SQL column reference.
    /// </summary>
    /// <param name="memberExpression">The member expression to translate.</param>
    protected override void Visit(MemberExpression memberExpression)
    {
        if (memberExpression is { Expression: ParameterExpression parameterExpression })
        {
            Append($"{Composite.GetAliasMapping(parameterExpression.Type)}.{memberExpression.Member.Name}");
        }
        else
        {
            throw new NotSupportedException("Expression not supported.");
        }
    }

    /// <summary>
    ///     Translates a unary expression into SQL for aggregate operations.
    ///     Handles operations like negation and type conversion.
    /// </summary>
    /// <param name="unaryExpression">The unary expression to translate.</param>
    protected override void Visit(UnaryExpression unaryExpression)
    {
        if (unaryExpression is { Operand: { } expression })
        {
            Visit(expression);
        }
    }
}
