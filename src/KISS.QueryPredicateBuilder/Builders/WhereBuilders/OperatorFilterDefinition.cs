namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

/// <summary>
/// A builder for an COMPARISON Operator
/// Create compare values in a database and determine
/// if they are equal, not  equal, greater than, less than, greater  than or equal to, and less than or equal to.
/// </summary>
/// <param name="ComparisonOperator">The COMPARISON Operator</param>
public sealed record OperatorFilterDefinition(FormattableString ComparisonOperator) : IComponent
{
    /// <summary>
    /// Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}