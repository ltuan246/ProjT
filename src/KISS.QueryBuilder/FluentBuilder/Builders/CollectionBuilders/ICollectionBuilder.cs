namespace KISS.QueryBuilder.FluentBuilder.Builders.CollectionBuilders;

/// <summary>
///     An interface that defines the collection builder type.
/// </summary>
/// <typeparam name="TRecordset">The type in the recordset.</typeparam>
public interface ICollectionBuilder<TRecordset>
{
    /// <summary>
    ///     Retrieves data from a database based on conditions.
    /// </summary>
    /// <returns>Retrieve the data based on conditions.</returns>
    IList<TRecordset> ToList();
}
