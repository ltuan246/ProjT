namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>FROM</c> clause.
/// </summary>
/// <param name="recordset">The type representing the database record set.</param>
internal sealed class SelectFromComponent(Type recordset) : QueryComponent
{
    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        Append("FROM");
        AppendLine(true);
        Append($"{recordset.Name}s {GetAliasMapping(recordset)}");
        AppendLine();

        visitor.Visit(this);
    }
}
