namespace KISS.QueryBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface for building SQL select queries.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface ISelectBuilder<TRecordset> : IJoinBuilder<TRecordset>, IGroupByBuilder<TRecordset>;