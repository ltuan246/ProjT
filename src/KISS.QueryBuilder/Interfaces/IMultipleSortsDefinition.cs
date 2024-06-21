namespace KISS.QueryBuilder.Interfaces;

public interface IMultipleSortsDefinition : IQuerying
{
    IEnumerable<DirectionalSortDefinition> Sorts { get; }
}