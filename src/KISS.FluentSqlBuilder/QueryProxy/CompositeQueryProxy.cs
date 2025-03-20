namespace KISS.FluentSqlBuilder.QueryProxy;

/// <summary>
///     A generic proxy class that intercepts method calls for <see cref="CompositeQuery" /> instances,
///     using the <see cref="DispatchProxy" /> mechanism to provide additional behavior. This class
///     enables dynamic query building and execution by intercepting calls to query operations.
/// </summary>
/// <typeparam name="TReturn">The type of result returned by the proxied operations.</typeparam>
public class CompositeQueryProxy<TReturn> : DispatchProxy
{
    /// <summary>
    ///     Holds the target <see cref="CompositeQuery" /> instance that this proxy delegates to.
    ///     This property stores the actual query implementation that will be executed.
    /// </summary>
    private CompositeQuery Composite { get; set; } = default!;

    /// <summary>
    ///     Creates a proxy instance for <see cref="ICompositeQueryOperations" /> that wraps a
    ///     <see cref="CompositeQuery" />. This method sets up the complete query execution
    ///     pipeline with proper configuration and interception.
    /// </summary>
    /// <param name="connection">The database connection used to initialize the <see cref="CompositeQuery" />.</param>
    /// <param name="handler">The handler that configures the query before proxy creation.</param>
    /// <returns>A proxied instance implementing <see cref="ICompositeQueryOperations" />.</returns>
    public ICompositeQueryOperations Create(DbConnection connection, QueryHandler handler)
    {
        // Instantiates a new CompositeQuery with the provided database connection.
        var composite = new CompositeQuery(connection);
        // Applies the handler's configuration to the newly created CompositeQuery instance.
        handler.Handle(composite);

        // Creates a DispatchProxy instance that implements ICompositeQueryOperations, using this class as the proxy type.
        var proxy = Create<ICompositeQueryOperations, CompositeQueryProxy<TReturn>>();
        // Casts the proxy to CompositeQueryProxy<TReturn> to access its properties for configuration.
        var dispatchProxy = (CompositeQueryProxy<TReturn>)proxy;
        // Assigns the configured CompositeQuery instance to the proxy's Composite property for later invocation.
        dispatchProxy.Composite = composite;

        // Returns the proxy, which now intercepts calls to ICompositeQueryOperations methods.
        return proxy;
    }

    /// <summary>
    ///     Intercepts method calls, ensures query setup is performed, and delegates execution
    ///     to the real service. This method is called automatically for all method invocations
    ///     on the proxy.
    /// </summary>
    /// <param name="targetMethod">The method being invoked on the <see cref="ICompositeQueryOperations" /> interface.</param>
    /// <param name="args">The arguments passed to the method during invocation.</param>
    /// <returns>The result of the method invocation, or null if the method returns void.</returns>
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        // Prepares the CompositeQuery by setting up its queries before the method is invoked.
        Composite.SetQueries();
        // Invokes the target method on the Composite instance, passing the provided arguments, and returns the result.
        return targetMethod!.Invoke(Composite, args);
    }
}
