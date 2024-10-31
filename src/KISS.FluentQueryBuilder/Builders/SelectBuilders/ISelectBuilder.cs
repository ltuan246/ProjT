namespace KISS.FluentQueryBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface that defines the select builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface ISelectBuilder<TEntity> : IJoinBuilder<TEntity>;
