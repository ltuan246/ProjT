namespace KISS.QueryPredicateBuilder.Core;

/// <summary>
///     The Component interface declares an `accept` method that should take the
///     base visitor interface as an argument.
/// </summary>
public interface IComponent
{
    /// <summary>
    ///     Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    void Accept(IVisitor visitor);
}
