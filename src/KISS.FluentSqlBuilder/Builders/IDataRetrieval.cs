namespace KISS.FluentSqlBuilder.Builders;

/// <summary>
///     Defines the contract for data retrieval operations.
///     This interface is responsible for retrieving various sets of data
///     through methods that apply common query setup logic and return
///     the results in different formats.
/// </summary>
public interface IDataRetrieval
{
    /// <summary>
    ///     Setup the query components.
    /// </summary>
    void SetQueries();

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    List<TReturn> GetGroupMap<TReturn>();

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    List<TReturn> GetSingleMap<TReturn>();

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    List<TReturn> GetMultiMap<TReturn>();
}
