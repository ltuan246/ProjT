namespace KISS.QueryPredicateBuilder.Builders.Common;

/// <summary>
///     The State is used to keep track of the method that the Visitor is currently executing.
/// </summary>
public sealed class ClauseState
{
    /// <summary>
    ///     The Clause is currently executing.
    /// </summary>
    public ClauseAction Context { get; init; }

    /// <summary>
    ///     The StringBuilder is currently combining.
    /// </summary>
    public StringBuilder Builder { get; } = new();

    /// <summary>
    ///     The StringBuilder is currently combining.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    ///     The total of the State executing.
    /// </summary>
    public int Length { get; init; }

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    public bool HasOpenParentheses { get; set; }
}
