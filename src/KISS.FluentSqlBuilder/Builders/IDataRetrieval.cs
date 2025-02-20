namespace KISS.FluentSqlBuilder.Builders;

/// <summary>
///     Defines the contract for data retrieval operations.
///     This interface is responsible for retrieving various sets of data
///     through methods that apply common query setup logic and return
///     the results in different formats.
/// </summary>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IDataRetrieval<TReturn>
{
    /// <summary>
    ///     Setup the query components.
    /// </summary>
    void SetQueries();

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <returns>Retrieve the data based on conditions.</returns>
    List<TReturn> GetGroupMap();

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <returns>Retrieve the data based on conditions.</returns>
    List<TReturn> GetSingleMap();

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <returns>Retrieve the data based on conditions.</returns>
    List<TReturn> GetMultiMap();
}
