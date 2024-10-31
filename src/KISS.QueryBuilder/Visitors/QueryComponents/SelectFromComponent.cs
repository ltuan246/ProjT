namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>FROM</c> clause.
/// </summary>
/// <param name="Recordset">The type representing the database record set.</param>
internal sealed record SelectFromComponent(Type Recordset) : IQueryComponent
{
    /// <summary>
    ///     Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    public void Accept(IVisitor visitor) => visitor.Visit(this);
}
