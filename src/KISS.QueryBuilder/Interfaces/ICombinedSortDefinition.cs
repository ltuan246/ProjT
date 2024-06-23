namespace KISS.QueryBuilder.Interfaces;

public interface ICombinedSortDefinition : IQuerying
{
    IEnumerable<DirectionalSortDefinition> Sorts { get; }
}