namespace KISS.QueryBuilder.Builders.OrderByBuilders;

/// <summary>
///     An interface for building SQL ORDER BY clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface IOrderByBuilder<TRecordset> : ILimitBuilder<TRecordset>;