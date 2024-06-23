namespace KISS.QueryBuilder.Interfaces;

public interface ISliceProjectionDefinition : IQuerying
{
    int Limit { get; init; }
    int Skip { get; init; }
}