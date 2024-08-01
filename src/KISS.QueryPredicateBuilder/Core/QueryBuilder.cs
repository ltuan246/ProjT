namespace KISS.QueryPredicateBuilder.Core;

/// <summary>
/// A class that defines the SQL builder type. The core QueryBuilder partial class.
/// </summary>
public sealed partial class QueryBuilder
{
    private ConcurrentDictionary<ClauseAction, StringBuilder> Builder { get; } = new();

    private SqlFormatter Formatter { get; } = new();

    private Stack<ClauseState> State { get; } = new();

    private ClauseState CurrentState => State.Peek();

    private StringBuilder StateBuilder => CurrentState.Builder;

    private bool HasOpenParentheses
    {
        get => CurrentState.HasOpenParentheses;
        set => CurrentState.HasOpenParentheses = value;
    }

    private int Length => CurrentState.Length;

    private void PushState(ClauseAction context, int length = 1)
    {
        ClauseState newState = new() { Context = context, Position = 0, Length = length };
        State.Push(newState);
    }

    private void PopState()
    {
        var recentState = State.Pop();
        _ = State.TryPeek(out _) switch
        {
            true => StateBuilder.Append(recentState.Builder),
            false => Builder.GetOrAdd(recentState.Context, recentState.Builder)
            // false => Builder.AddOrUpdate(recentState.Context, recentState.Builder, (_, built) => built.Append(recentState.Builder))
        };
    }

    private void Join(string separator, IEnumerable<IComponent> expressions)
    {
        using IEnumerator<IComponent> enumerator = expressions.GetEnumerator();
        if (enumerator.MoveNext())
        {
            OpenParentheses();
            enumerator.Current.Accept(this);
            while (enumerator.MoveNext())
            {
                ++CurrentState.Position;
                StateBuilder.Append(separator);
                enumerator.Current.Accept(this);
            }

            CloseOpenParentheses();
        }
    }

    private void OpenParentheses()
    {
        HasOpenParentheses = Length > 1;
        if (HasOpenParentheses)
        {
            StateBuilder.Append(ClauseConstants.OpenParentheses);
        }
    }

    private void CloseOpenParentheses()
    {
        if (HasOpenParentheses)
        {
            StateBuilder.Append(ClauseConstants.CloseParentheses);
            HasOpenParentheses = false;
        }
    }
}
