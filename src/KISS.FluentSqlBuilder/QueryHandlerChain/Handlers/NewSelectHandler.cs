namespace KISS.FluentSqlBuilder.QueryHandlerChain.Handlers;

/// <summary>
///     SelectHandler.
/// </summary>
/// <typeparam name="TSource">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed partial record NewSelectHandler<TSource, TReturn>(Expression Selector) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process() { }
}