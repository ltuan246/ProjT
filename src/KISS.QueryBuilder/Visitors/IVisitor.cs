namespace KISS.QueryBuilder.Visitors;

/// <summary>
///     The Visitor Interface declares a set of visiting methods that correspond
///     to component classes. The signature of a visiting method allows the
///     visitor to identify the exact class of the component that it's dealing with.
/// </summary>
internal interface IVisitor
{
    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The SelectComponent.</param>
    void Visit(SelectComponent element);

    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The SelectFromComponent.</param>
    void Visit(SelectFromComponent element);

    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The JoinComponent.</param>
    void Visit(JoinComponent element);

    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The WhereComponent.</param>
    void Visit(WhereComponent element);

    /// <summary>
    ///     Visit methods for <paramref name="element" />.
    /// </summary>
    /// <param name="element">The GroupByComponent.</param>
    void Visit(GroupByComponent element);
}
