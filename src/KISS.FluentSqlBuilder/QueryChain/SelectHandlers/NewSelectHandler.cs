namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     A handler for processing SELECT clauses with object initialization in a query chain.
///     This class is responsible for generating SQL statements for selecting and mapping
///     data to newly created objects using object initializers.
/// </summary>
/// <typeparam name="TSource">
///     The type representing the database record set.
///     This type defines the structure of the data being queried.
/// </typeparam>
/// <typeparam name="TReturn">
///     The type of the object to be created and returned.
///     This type defines the structure of the result object.
/// </typeparam>
/// <param name="Selector">
///     An expression defining how to create and initialize the return object.
///     This expression typically contains object initialization syntax.
/// </param>
public sealed partial record NewSelectHandler<TSource, TReturn>(Expression Selector)
    : QueryHandler(SqlStatement.Select, Selector)
{
    /// <summary>
    ///     Processes the SELECT clause with object initialization.
    ///     This method is implemented in the translator class to handle the actual
    ///     translation of the object initialization expression into SQL.
    /// </summary>
    protected override void TranslateExpression()
    {
        Visit(Selector);
        var sql = SqlBuilder.ToString();
        _ = sql;
    }
}
