namespace KISS.QueryBuilder.Interfaces;

public interface IQueryBuilders : IQuerying
{
    IEnumerable<IQuerying> Queries { get; }
}