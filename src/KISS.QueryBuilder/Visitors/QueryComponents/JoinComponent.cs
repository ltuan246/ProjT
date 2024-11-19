namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>JOIN</c> clause.
/// </summary>
/// <param name="tableAliases">A collection specifically for table aliases.</param>
internal sealed class JoinComponent(Dictionary<Type, string> tableAliases) : QueryComponent
{
    /// <inheritdoc />
    protected override Dictionary<Type, string> TableAliases { get; } = tableAliases;

    /// <summary>
    ///     A collection of the types of tables you want to join.
    /// </summary>
    public List<(Type Recordset, Expression LeftKeySelector, Expression RightKeySelector)> Joins { get; } = [];

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        using var enumerator = Joins.GetEnumerator();
        if (enumerator.MoveNext())
        {
            Append("INNER JOIN");
            Append($" {enumerator.Current.Recordset.Name}s {GetAliasMapping(enumerator.Current.Recordset)} ");
            Append("ON ");
            Translate(enumerator.Current.LeftKeySelector);
            Append(" = ");
            Translate(enumerator.Current.RightKeySelector);

            while (enumerator.MoveNext())
            {
                AppendLine();
                Append("INNER JOIN");
                Append($" {enumerator.Current.Recordset.Name}s {GetAliasMapping(enumerator.Current.Recordset)} ");
                Append("ON ");
                Translate(enumerator.Current.LeftKeySelector);
                Append(" = ");
                Translate(enumerator.Current.RightKeySelector);
            }

            AppendLine();
        }

        visitor.Visit(this);
    }

    /// <inheritdoc />
    protected override void Translate(MemberExpression memberExpression)
    {
        switch (memberExpression.Expression)
        {
            // Accessing a property or field of a parameter in a lambda
            case ParameterExpression parameterExpression:
                {
                    Append($"{GetAliasMapping(parameterExpression.Type)}.{memberExpression.Member.Name}");
                    break;
                }
        }
    }
}
