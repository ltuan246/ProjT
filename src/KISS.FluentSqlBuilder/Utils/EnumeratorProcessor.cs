namespace KISS.FluentSqlBuilder.Utils;

/// <summary>
///     A generic class that processes an <see cref="IEnumerable{T}" /> by applying custom actions to the first item,
///     remaining items, and after the last item. It uses a fluent API to configure the actions and executes them when
///     <see cref="Execute" /> is called.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
/// <param name="Collection">The collection to process.</param>
public record EnumeratorProcessor<T>(IEnumerable<T> Collection)
{
    /// <summary>
    ///     The action to apply to the first item in the collection.
    /// </summary>
    private Action<T>? FirstAction { get; set; }

    /// <summary>
    ///     The action to apply to each remaining item in the collection (after the first).
    /// </summary>
    private Action<T>? RemainingAction { get; set; }

    /// <summary>
    ///     The action to apply after processing the last item in the collection.
    /// </summary>
    private Action? LastAction { get; set; }

    /// <summary>
    ///     Sets the action to be applied to the first item in the collection.
    /// </summary>
    /// <param name="firstAction">The action to apply to the first item.</param>
    /// <returns>This instance for method chaining.</returns>
    public EnumeratorProcessor<T> AccessFirst(Action<T> firstAction)
    {
        FirstAction = firstAction;
        return this;
    }

    /// <summary>
    ///     Sets the action to be applied to each remaining item in the collection (after the first).
    /// </summary>
    /// <param name="remainingAction">The action to apply to each remaining item.</param>
    /// <returns>This instance for method chaining.</returns>
    public EnumeratorProcessor<T> AccessRemaining(Action<T> remainingAction)
    {
        RemainingAction = remainingAction;
        return this;
    }

    /// <summary>
    ///     Sets the action to be applied after processing the last item in the collection.
    /// </summary>
    /// <param name="lastAction">The action to apply after the last item.</param>
    /// <returns>This instance for method chaining.</returns>
    public EnumeratorProcessor<T> AccessLast(Action lastAction)
    {
        LastAction = lastAction;
        return this;
    }

    /// <summary>
    ///     Executes the configured actions on the collection:
    ///     - Applies <see cref="FirstAction" /> to the first item.
    ///     - Applies <see cref="RemainingAction" /> to each subsequent item.
    ///     - Applies <see cref="LastAction" /> after processing the last item.
    ///     If the collection is empty, no actions are applied.
    /// </summary>
    public void Execute()
    {
        using var itor = Collection.GetEnumerator();
        if (itor.MoveNext())
        {
            FirstAction?.Invoke(itor.Current);

            if (RemainingAction is not null)
            {
                while (itor.MoveNext())
                {
                    RemainingAction.Invoke(itor.Current);
                }
            }

            LastAction?.Invoke();
        }
    }
}
