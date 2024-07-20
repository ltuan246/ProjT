namespace KISS.QueryPredicateBuilder.Builders.OffsetBuilders;

/// <summary>
/// A builder for a OFFSET Clause
/// </summary>
/// <param name="OffsetClause">The OFFSET clause.</param>
public sealed record OffsetDefinition(FormattableString OffsetClause) : IComponent
{
    /// <summary>
    /// Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}