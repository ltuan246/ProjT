namespace KISS.FluentQueryBuilder.Builders.OrderByBuilders;

/// <summary>
///     An interface that defines the order by builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IOrderByBuilder<TEntity> :
    IOrderByBuilderEntry<TEntity>,
    IFetchBuilder<TEntity>,
    IOffsetBuilder<TEntity>,
    IOffsetRowsBuilder<TEntity>,
    ILimitBuilder<TEntity>;
