namespace KISS.FluentQueryBuilder.Builders;

public sealed partial class FluentBuilder
{
    private StringBuilder StringBuilder { get; } = new();

    private SqlFormatter Formatter { get; } = new();

    private Stack<BuildState> State { get; } = new();

    // private BuildState CurrentState => State.Peek();

    private void PushState(Expression expression)
    {
        BuildState newState = new() { Expression = expression };
        State.Push(newState);
    }

    private void PopState()
    {
        _ = State.Pop();
    }

    private sealed class BuildState
    {
        public required Expression Expression { get; init; }
    }
}
