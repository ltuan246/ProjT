namespace KISS.QueryBuilder.Interfaces;

public interface ICombinedProjectionDefinition : IQuerying
{
    IQuerying[] Projections { get; }
}