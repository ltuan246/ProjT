namespace KISS.GuardClauses;

public interface IGuardClause;

public sealed record Guard : IGuardClause
{
    public static IGuardClause Against { get; } = new Guard();

    private Guard() { }
}