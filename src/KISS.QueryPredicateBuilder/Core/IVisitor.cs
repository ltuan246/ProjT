namespace KISS.QueryPredicateBuilder.Core;

/// <summary>
///     The Visitor Interface declares a set of visiting methods that correspond
///     to component classes. The signature of a visiting method allows the
///     visitor to identify the exact class of the component that it's dealing with.
/// </summary>
public interface IVisitor
{
    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The OperatorFilterDefinition.</param>
    void Visit(OperatorFilterDefinition element);

    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The SingleItemAsArrayOperatorFilterDefinition.</param>
    void Visit(SingleItemAsArrayOperatorFilterDefinition element);

    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The RangeFilterDefinition.</param>
    void Visit(RangeFilterDefinition element);

    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The CombinedFilterDefinition.</param>
    void Visit(CombinedFilterDefinition element);

    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The ProjectionDefinition.</param>
    void Visit(ProjectionDefinition element);

    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The OffsetDefinition.</param>
    void Visit(OffsetDefinition element);

    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The FetchDefinition.</param>
    void Visit(FetchDefinition element);

    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The OrderByDefinition.</param>
    void Visit(OrderByDefinition element);
}
