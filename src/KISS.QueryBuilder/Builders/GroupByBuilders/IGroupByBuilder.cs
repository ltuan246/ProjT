namespace KISS.QueryBuilder.Builders.GroupByBuilders;

/// <summary>
///     An interface for building SQL GROUP BY clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface IGroupByBuilder<TRecordset> : IAggregateBuilder<TRecordset>;
