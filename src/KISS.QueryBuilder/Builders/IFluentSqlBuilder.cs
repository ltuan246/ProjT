namespace KISS.QueryBuilder.Builders;

/// <summary>
///     An interface for building a fluent SQL query.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface IFluentSqlBuilder<TRecordset>
{
    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <returns>Retrieve the data based on conditions.</returns>
    List<TRecordset> ToList();
}
