namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

/// <summary>
///     A builder for a BETWEEN Operator
///     Create a query specifying a range of values to be return.
/// </summary>
/// <param name="BetweenOperator">The BETWEEN Operator.</param>
public sealed record RangeFilterDefinition(FormattableString BetweenOperator) : IComponent
{
    /// <summary>
    ///     Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}
