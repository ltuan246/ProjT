namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

/// <summary>
/// A builder for a (Does not) Contain element.
/// </summary>
/// <param name="ContainsClause">The (NOT) IN Operator.</param>
public sealed record SingleItemAsArrayOperatorFilterDefinition(FormattableString ContainsClause) : IComponent
{
    /// <summary>
    /// Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}
