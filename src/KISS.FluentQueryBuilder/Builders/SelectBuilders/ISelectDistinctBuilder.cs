namespace KISS.FluentQueryBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface that defines the select distinct builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface ISelectDistinctBuilder<TEntity> : ISelectDistinctBuilderEntry<TEntity>, IWhereBuilderEntry<TEntity>;
