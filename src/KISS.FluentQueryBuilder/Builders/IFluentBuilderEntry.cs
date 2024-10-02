namespace KISS.FluentQueryBuilder.Builders;

/// <summary>
///     An interface that defines the fluent builder entry type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IFluentBuilderEntry<TEntity> :
    ISelectBuilderEntry<TEntity>,
    ISelectDistinctBuilderEntry<TEntity>,
    IWhereBuilderEntry<TEntity>,
    IOrderByBuilderEntry<TEntity>;
