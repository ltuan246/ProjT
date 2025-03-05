namespace KISS.FluentSqlBuilder.Builders;

/// <summary>
///     An interface that defines the fluent SQL builder type.
/// </summary>
public interface ISqlBuilder;

/// <summary>
///     An interface that defines the fluent SQL builder type.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <returns>Retrieve the data based on conditions.</returns>
    List<TReturn> ToList();
}

/// <summary>
///     An interface that defines the fluent SQL builder type.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISqlBuilder<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <returns>Retrieve the data based on conditions.</returns>
    List<TReturn> ToList();
}

/// <summary>
///     An interface that defines the fluent SQL builder type.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISqlBuilder<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <returns>Retrieve the data based on conditions.</returns>
    List<TReturn> ToList();
}

/// <summary>
///     An interface that defines the fluent SQL builder type.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IGroupSqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <returns>Retrieve the data based on conditions.</returns>
    Dictionary<ITuple, List<TReturn>> ToDictionary();
}
