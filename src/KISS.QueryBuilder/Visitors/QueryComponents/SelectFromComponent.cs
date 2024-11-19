namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>FROM</c> clause.
/// </summary>
/// <param name="recordset">The type representing the database record set.</param>
internal sealed class SelectFromComponent(MemberInfo recordset) : QueryComponent
{
    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        Append("FROM");
        AppendLine(true);
        Append($"{recordset.Name}s {ClauseConstants.DefaultTableAlias}{0}");
        AppendLine();

        visitor.Visit(this);
    }
}
