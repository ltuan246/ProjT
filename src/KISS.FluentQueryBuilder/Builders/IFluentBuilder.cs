namespace KISS.FluentQueryBuilder.Builders;

/// <summary>
///     An interface that defines the fluent builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IFluentBuilder<TEntity> :
    ISelectBuilder<TEntity>,
    ISelectDistinctBuilder<TEntity>,
    IOrderByBuilder<TEntity>,
    IFetchBuilder<TEntity>,
    IOffsetRowsBuilder<TEntity>,
    ILimitBuilder<TEntity>,
    IOffsetBuilder<TEntity>;
