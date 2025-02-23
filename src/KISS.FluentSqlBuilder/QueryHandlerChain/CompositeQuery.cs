namespace KISS.FluentSqlBuilder.QueryHandlerChain;

/// <summary>
///     CompositeQuery.
/// </summary>
public sealed partial class CompositeQuery(DbConnection connection)
{
    /// <summary>
    ///     The database connections.
    /// </summary>
    private DbConnection Connection { get; } = connection;

    private QueryHandler? FirstHandler { get; set; }

    /// <summary>
    ///     AddHandler.
    /// </summary>
    /// <param name="handler">QueryHandler.</param>
    public void AddHandler(QueryHandler handler)
    {
        if (FirstHandler is null)
        {
            FirstHandler = handler;
        }
        else
        {
            var lastHandler = FirstHandler;
            while (lastHandler.NextHandler is not null)
            {
                lastHandler = lastHandler.NextHandler;
            }

            lastHandler.SetNext(handler);
        }
    }

    /// <summary>
    ///     AddHandler.
    /// </summary>
    public void GetList()
    {
        FirstHandler?.Handle(this);
        _ = Connection.Query(Sql, Parameters)
            .Cast<IDictionary<string, object>>()
            .ToList();
    }
}
