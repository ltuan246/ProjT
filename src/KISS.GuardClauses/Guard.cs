namespace KISS.GuardClauses;

/// <summary>
///     An entry point to a set of Guard Clauses defined as extension methods on IGuardClause.
/// </summary>
public sealed record Guard : IGuardClause
{
    private Guard() { }

    /// <summary>
    ///     An entry point to a set of Guard Clauses.
    /// </summary>
    public static IGuardClause Against { get; } = new Guard();
}
