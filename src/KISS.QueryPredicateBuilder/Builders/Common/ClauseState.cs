namespace KISS.QueryPredicateBuilder.Builders.Common;

public sealed class ClauseState
{
    public ClauseAction Context { get; init; }

    public StringBuilder Builder { get; } = new();

    public int Position { get; set; }

    public int Length { get; set; }

    public bool HasOpenParentheses { get; set; }
}