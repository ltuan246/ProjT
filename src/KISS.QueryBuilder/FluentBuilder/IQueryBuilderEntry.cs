namespace KISS.QueryBuilder.FluentBuilder;

/// <summary>
///     An interface that defines the fluent builder entry type.
/// </summary>
/// <typeparam name="TRecordset">The type in the recordset.</typeparam>
public interface IQueryBuilderEntry<TRecordset> :
    ISelectBuilderEntry<TRecordset>;
