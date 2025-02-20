namespace KISS.FluentSqlBuilder.Builders;

/// <summary>
///     A dynamic proxy class that intercepts method calls to an underlying <see cref="IDataRetrieval" /> service.
///     This class ensures that common query setup logic (such as <see cref="IDataRetrieval.SetQueries" />)
///     is executed before delegating the method call to the actual service implementation.
/// </summary>
/// <remarks>
///     This proxy leverages <see cref="DispatchProxy" /> to dynamically create an instance of <see cref="IDataRetrieval" />.
///     It applies pre-processing logic, such as setting up queries, before forwarding the invocation to the target method.
/// </remarks>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public class DataRetrievalDispatchProxy<TReturn> : DispatchProxy
{
    /// <summary>
    ///     Gets the underlying data retrieval service that this proxy delegates to.
    /// </summary>
    private IDataRetrieval<TReturn>? DataRetrieval { get; set; }

    /// <summary>
    ///     Creates a proxy instance of <see cref="IDataRetrieval" />, wrapping it with
    ///     <see cref="DataRetrievalDispatchProxy" />.
    /// </summary>
    /// <param name="dataRetrieval">The underlying data retrieval service that this proxy delegates to.</param>
    /// <returns>A proxied instance of <see cref="IDataRetrieval" />.</returns>
    public IDataRetrieval<TReturn> Create(IDataRetrieval<TReturn> dataRetrieval)
    {
        var proxy = Create<IDataRetrieval<TReturn>, DataRetrievalDispatchProxy<TReturn>>();
        var dispatchProxy = (DataRetrievalDispatchProxy<TReturn>)proxy;
        dispatchProxy.DataRetrieval = dataRetrieval;
        return proxy;
    }

    /// <summary>
    ///     Intercepts method calls, ensures query setup is performed, and delegates execution to the real service.
    /// </summary>
    /// <param name="targetMethod">The method being invoked.</param>
    /// <param name="args">The arguments passed to the method.</param>
    /// <returns>The result of the method invocation.</returns>
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        DataRetrieval!.SetQueries();
        return targetMethod!.Invoke(DataRetrieval, args);
    }
}
