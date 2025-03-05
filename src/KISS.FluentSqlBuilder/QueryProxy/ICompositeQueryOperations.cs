namespace KISS.FluentSqlBuilder.QueryProxy;

/// <summary>
///     Defines the contract for data retrieval operations.
///     This interface is responsible for retrieving various sets of data
///     through methods that apply common query setup logic and return
///     the results in different formats.
/// </summary>
public interface ICompositeQueryOperations
{
    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    List<TReturn> GetList<TReturn>();

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    Dictionary<ITuple, List<TReturn>> GetDictionary<TReturn>();
}
