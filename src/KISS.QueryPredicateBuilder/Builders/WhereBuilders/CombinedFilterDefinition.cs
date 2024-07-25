namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

/// <summary>
/// A builder for a LOGICAL Operators.
/// AND – Returns true if both conditions are True.
/// OR – Returns true if either of the conditions is True.
/// NOT – Returns true if the condition is false.
/// </summary>
/// <param name="Clause">The type of clause</param>
/// <param name="Separator">The separator</param>
/// <param name="Operators">The operators</param>
public sealed record CombinedFilterDefinition(ClauseAction Clause, string Separator, IReadOnlyList<IComponent> Operators) : IComponent
{
    /// <summary>
    /// Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}