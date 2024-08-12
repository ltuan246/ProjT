namespace KISS.QueryPredicateBuilder.Builders.SelectBuilders;

/// <summary>
/// A builder for a Projection clause.
/// </summary>
/// <param name="ProjectionClause">The Projection Clause.</param>
public sealed record ProjectionDefinition(FormattableString ProjectionClause) : IComponent
{
    /// <summary>
    /// Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}
