namespace KISS.FluentSqlBuilder.QueryProxy;

/// <summary>
///     A generic proxy class that intercepts method calls for <see cref="IComposite" /> instances,
///     using the <see cref="DispatchProxy" /> mechanism to provide additional behavior. This class
///     enables dynamic query building and execution by intercepting calls to query operations.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">The type of result returned by the proxied operations.</typeparam>
public class CompositeQueryProxy<TRecordset, TReturn> : DispatchProxy
{
    /// <summary>
    ///     Holds the target <see cref="IComposite" /> instance that this proxy delegates to.
    ///     This property stores the actual query implementation that will be executed.
    /// </summary>
    private QueryOperator<TRecordset, TReturn> Operator { get; set; } = default!;

    /// <summary>
    ///     Creates a proxy instance for <see cref="ICompositeQueryOperations{TReturn}" /> that wraps a
    ///     <see cref="IComposite" />. This method sets up the complete query execution
    ///     pipeline with proper configuration and interception.
    /// </summary>
    /// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
    /// <param name="connection">The database connection used to initialize the <see cref="IComposite" />.</param>
    /// <param name="handler">The handler that configures the query before proxy creation.</param>
    /// <returns>A proxied instance implementing <see cref="ICompositeQueryOperations{TReturn}" />.</returns>
    public ICompositeQueryOperations<TRecordset, TReturn> Create(DbConnection connection, QueryHandler handler)
    {
        // Instantiates a new CompositeQuery with the provided database connection.
        IComposite composite = new CompositeQuery<TRecordset, TReturn>();

        // Applies the handler's configuration to the newly created CompositeQuery instance.
        composite = handler.Handle(composite);

        var sql = composite.Sql;
        System.Diagnostics.Debug.WriteLine(sql);

        // Executes the SQL query using the Connection, passing the constructed Sql string and Parameters
        var dtRows = connection.Query(sql, composite.Parameters)
            .Cast<IDictionary<string, object>>()
            .ToList();

        QueryOperator<TRecordset, TReturn> queryOperator = new(handler, composite, dtRows);

        // Creates a DispatchProxy instance that implements ICompositeQueryOperations, using this class as the proxy type.
        var proxy = Create<ICompositeQueryOperations<TRecordset, TReturn>, CompositeQueryProxy<TRecordset, TReturn>>();
        // Casts the proxy to CompositeQueryProxy<TReturn> to access its properties for configuration.
        var dispatchProxy = (CompositeQueryProxy<TRecordset, TReturn>)proxy;
        // Assigns the configured CompositeQuery instance to the proxy's Composite property for later invocation.
        dispatchProxy.Operator = queryOperator;

        // Returns the proxy, which now intercepts calls to ICompositeQueryOperations methods.
        return proxy;
    }

    /// <summary>
    ///     Intercepts method calls, ensures query setup is performed, and delegates execution
    ///     to the real service. This method is called automatically for all method invocations
    ///     on the proxy.
    /// </summary>
    /// <param name="targetMethod">The method being invoked on the <see cref="ICompositeQueryOperations{TReturn}" /> interface.</param>
    /// <param name="args">The arguments passed to the method during invocation.</param>
    /// <returns>The result of the method invocation, or null if the method returns void.</returns>
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        => targetMethod!.Invoke(Operator, args);
}
