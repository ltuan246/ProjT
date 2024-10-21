namespace KISS.QueryBuilder.FluentBuilder;

/// <summary>
///     An interface that defines the fluent builder type.
/// </summary>
/// <typeparam name="TRecordset">The type in the recordset.</typeparam>
public interface IQueryBuilder<TRecordset> :
    ISelectBuilder<TRecordset>,
    IJoinBuilder<TRecordset>,
    IWhereBuilder,
    IOrderByBuilder,
    IFetchBuilder,
    IOffsetRowsBuilder,
    ILimitBuilder,
    IOffsetBuilder,
    ICollectionBuilder<TRecordset>;
