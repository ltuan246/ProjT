namespace KISS.QueryBuilder.FluentBuilder.Builders.OrderByBuilders;

/// <summary>
///     An interface that defines the order by builder type.
/// </summary>
public interface IOrderByBuilder : IOrderByBuilderEntry, IFetchBuilder, IOffsetRowsBuilder, ILimitBuilder;
