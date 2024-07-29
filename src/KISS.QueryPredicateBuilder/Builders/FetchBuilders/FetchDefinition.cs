namespace KISS.QueryPredicateBuilder.Builders.FetchBuilders;

/// <summary>
/// A builder for a FETCH Clause.
/// </summary>
/// <param name="FetchClause">The FETCH clause.</param>
public sealed record FetchDefinition(FormattableString FetchClause) : IComponent
{
    /// <summary>
    /// Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}
