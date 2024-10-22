namespace KISS.QueryBuilder.FluentBuilder.Builders.OrderByBuilders;

/// <summary>
///     An interface that defines the order by builder type.
/// </summary>
/// <typeparam name="TRecordset">The type in the recordset.</typeparam>
public interface IOrderByBuilder<TRecordset> :
    IOrderByBuilderEntry<TRecordset>,
    IFetchBuilder,
    IOffsetRowsBuilder,
    ILimitBuilder;
