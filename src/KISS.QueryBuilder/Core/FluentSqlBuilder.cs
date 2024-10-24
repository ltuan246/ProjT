namespace KISS.QueryBuilder.Core;

/// <summary>
///     A class that defines the fluent SQL builder type.
///     The core <see cref="FluentSqlBuilder{TRecordset}" /> partial class.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public partial class FluentSqlBuilder<TRecordset>
{
    /// <summary>
    ///      Initializes a new instance of the <see cref="FluentSqlBuilder{TRecordset}"/> class.
    /// </summary>
    public FluentSqlBuilder()
    {
        var recType = typeof(TRecordset);
        var tableAlias = GetTableAlias(recType);
        SqlBuilder = new();
        Append($"{ClauseConstants.Select} {tableAlias}.* FROM {recType.Name}s {tableAlias} ");
    }

    /// <inheritdoc/>
    public string Sql
        => SqlBuilder.ToString();

    /// <inheritdoc />
    public DynamicParameters Parameters
        => SqlFormat.Parameters;

    /// <summary>
    ///     The connection to a database.
    /// </summary>
    public required DbConnection Connection { get; init; }

    private StringBuilder SqlBuilder { get; init; }

    private SqlFormatter SqlFormat { get; } = new();

    private Type RootTable { get; } = typeof(TRecordset);

    private Dictionary<Type, string> TableAliasesMap { get; } = new()
    {
        [typeof(TRecordset)] = $"{ClauseConstants.DefaultTableAlias}{0}"
    };

    private List<ClauseAction> ClauseActions { get; set; } = [];

    /// <summary>
    ///     Use checks to know when to use Distinct.
    /// </summary>
    private bool HasDistinct { get; set; }

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    private bool HasOpenParentheses { get; set; }

    private void OpenParentheses()
    {
        HasOpenParentheses = true;
        SqlBuilder.Append(ClauseConstants.OpenParenthesis);
    }

    private void CloseParentheses()
    {
        if (!HasOpenParentheses)
        {
            return;
        }

        HasOpenParentheses = false;
        SqlBuilder.Append(ClauseConstants.CloseParenthesis);
    }

    private string GetTableAlias(Type type)
    {
        if (!TableAliasesMap.TryGetValue(type, out var tableAlias))
        {
            tableAlias = $"{ClauseConstants.DefaultTableAlias}{TableAliasesMap.Count}";
            TableAliasesMap.Add(type, tableAlias);
        }

        return tableAlias;
    }
}
