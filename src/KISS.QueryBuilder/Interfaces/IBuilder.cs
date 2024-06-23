namespace KISS.QueryBuilder.Interfaces;

public interface IBuilder : IQuerying
{
    IEnumerable<IQuerying> Queries { get; }
}