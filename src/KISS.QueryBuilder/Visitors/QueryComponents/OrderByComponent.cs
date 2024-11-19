namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>ORDER BY</c> clause.
/// </summary>
/// <param name="tableAliases">A collection specifically for table aliases.</param>
internal sealed class OrderByComponent(Dictionary<Type, string> tableAliases) : QueryComponent
{
    /// <inheritdoc />
    protected override Dictionary<Type, string> TableAliases { get; } = tableAliases;

    /// <summary>
    ///     The table columns.
    /// </summary>
    public List<Expression> Selectors { get; } = [];

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        using var enumerator = Selectors.GetEnumerator();
        if (enumerator.MoveNext())
        {
            Append("ORDER BY");
            AppendLine(true);
            Translate(enumerator.Current);
            while (enumerator.MoveNext())
            {
                Append(", ");
                Translate(enumerator.Current);
            }

            AppendLine();
        }

        visitor.Visit(this);
    }

    /// <inheritdoc />
    protected override void Translate(UnaryExpression unaryExpression)
    {
        switch (unaryExpression.Operand)
        {
            // Accessing a property or field of a parameter in a lambda
            case MemberExpression memberExpression:
                {
                    Append($"{GetAliasMapping(memberExpression.Member.DeclaringType!)}.{memberExpression.Member.Name}");
                    break;
                }

            default:
                throw new NotSupportedException("Expression not supported.");
        }
    }
}
