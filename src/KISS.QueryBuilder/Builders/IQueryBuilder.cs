namespace KISS.QueryBuilder.Builders;

/// <summary>
///     An interface that defines the query-building method shared across clauses,
///     each corresponding to different SQL clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface IQueryBuilder<TRecordset> :
    ISelectBuilder<TRecordset>,
    ISelectBuilderEntry<TRecordset>,
    IJoinBuilder<TRecordset>,
    IWhereBuilder<TRecordset>,
    IGroupByBuilder<TRecordset>,
    IOrderByBuilder<TRecordset>,
    ILimitBuilder<TRecordset>,
    IOffsetBuilder<TRecordset>
    where TRecordset : IEntityBuilder;
