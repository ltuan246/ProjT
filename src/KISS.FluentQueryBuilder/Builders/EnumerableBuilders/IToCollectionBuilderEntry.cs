namespace KISS.FluentQueryBuilder.Builders.EnumerableBuilders;

/// <summary>
///     An interface that defines the standard query operators for querying data sources.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public interface IToCollectionBuilderEntry<TEntity>
{
    /// <summary>
    ///     Retrieves data from a database based on conditions.
    /// </summary>
    /// <returns>Retrieve the data based on conditions.</returns>
    IList<TEntity> ToList();
}
