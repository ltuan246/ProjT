namespace KISS.QueryBuilder.FluentBuilder;

/// <summary>
///     An interface that defines the simple fluent builder type.
/// </summary>
internal interface IQueryBuilder :
    ISelectBuilder,
    ISelectDistinctBuilder,
    IJoinBuilder,
    IWhereBuilder,
    IOrderByBuilder,
    IFetchBuilder,
    IOffsetRowsBuilder,
    ILimitBuilder,
    IOffsetBuilder;
