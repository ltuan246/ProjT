namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     A handler for processing <c>SELECT</c> in a query chain.
/// </summary>
/// <typeparam name="TSource">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed partial record NewSelectHandler<TSource, TReturn>(Expression Selector) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process() { }
}
