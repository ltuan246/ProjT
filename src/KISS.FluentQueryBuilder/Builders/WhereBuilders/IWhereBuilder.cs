namespace KISS.FluentQueryBuilder.Builders.WhereBuilders;

/// <summary>
///     An interface that defines the where builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IWhereBuilder<TEntity> : IToCollectionBuilderEntry<TEntity>;
