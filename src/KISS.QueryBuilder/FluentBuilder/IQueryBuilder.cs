namespace KISS.QueryBuilder.FluentBuilder;

/// <summary>
///     An interface that defines the fluent builder type.
/// </summary>
public interface IQueryBuilder :
    ISelectBuilder,
    ISelectDistinctBuilder,
    IJoinBuilder,
    IWhereBuilder,
    IOrderByBuilder,
    IFetchBuilder,
    IOffsetRowsBuilder,
    ILimitBuilder,
    IOffsetBuilder;
