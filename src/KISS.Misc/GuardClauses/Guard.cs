namespace KISS.Misc.GuardClauses;

public interface IGuardClause
{
}

public record Guard : IGuardClause
{
    public static IGuardClause Against { get; } = new Guard();

    private Guard() { }
}
