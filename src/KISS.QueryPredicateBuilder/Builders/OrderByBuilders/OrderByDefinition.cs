namespace KISS.QueryPredicateBuilder.Builders.OrderByBuilders;

/// <summary>
/// A builder for an ORDER BY clause.
/// </summary>
/// <param name="OrderByClause">The ORDER BY Clause.</param>
public sealed record OrderByDefinition(FormattableString OrderByClause) : IComponent
{
    /// <summary>
    /// Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}
