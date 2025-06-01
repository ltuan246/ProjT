namespace KISS.FluentSqlBuilder.QueryChain.JoinHandlers;

/// <summary>
///     Provides translation logic for join operations in a query chain, converting expression trees
///     into SQL JOIN statements and column references. Supports translation of binary, member, and unary
///     expressions for building SQL join conditions and projections.
/// </summary>
/// <typeparam name="TRelation">
///     The type of the relation (source table or entity).
///     Used for generating table names and aliases in SQL.
/// </typeparam>
/// <typeparam name="TReturn">
///     The combined type to return as the result of the join operation.
/// </typeparam>
public abstract partial record JoinHandler<TRelation, TReturn>
{
    /// <summary>
    ///     Translates a binary expression into SQL JOIN syntax.
    ///     Handles logical operations, comparisons, and array indexing for join conditions.
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
    ///     Translates a member expression into a SQL column reference for join queries.
    ///     Converts property or field access into the appropriate table alias and column name.
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
    ///     Translates a unary expression into SQL for join and aggregate operations.
    ///     Handles operations such as negation and type conversion by recursively visiting the operand.
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
