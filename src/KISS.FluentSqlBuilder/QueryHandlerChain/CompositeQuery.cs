namespace KISS.FluentSqlBuilder.QueryHandlerChain;

/// <summary>
///     CompositeQuery.
/// </summary>
public sealed partial class CompositeQuery(DbConnection connection)
{
    /// <summary>
    ///     The database connections.
    /// </summary>
    private DbConnection Connection { get; } = connection;

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    public List<TReturn> GetList<TReturn>()
    {
        using var selectItor = SqlStatements[SqlStatement.Select].GetEnumerator();
        if (selectItor.MoveNext())
        {
            Append("SELECT");
            AppendLine($"{selectItor.Current}");

            while (selectItor.MoveNext())
            {
                AppendLine($", {selectItor.Current}");
            }

            AppendLine();
        }

        Append("FROM");
        AppendLine($"{TableAliases.First().Key.Name}s AS {TableAliases.First().Value}");
        AppendLine();

        using var whereItor = SqlStatements[SqlStatement.Where].GetEnumerator();
        if (whereItor.MoveNext())
        {
            Append("WHERE");
            AppendLine($"{whereItor.Current}");

            while (whereItor.MoveNext())
            {
                AppendLine($"AND {whereItor.Current}");
            }

            AppendLine();
        }

        var dtRows = Connection.Query(Sql, Parameters)
            .Cast<IDictionary<string, object>>()
            .ToList();

        OutputProcessor = (p) => Expression.Block(
            [p.CurrentEntityVariable],
            Expression.Call(
                p.OutputCollectionVariable,
                typeof(List<TReturn>).GetMethod("Add")!,
                p.CurrentEntityVariable));

        var res = ProcessData<TReturn, List<TReturn>>(dtRows);

        return res;
    }
}
