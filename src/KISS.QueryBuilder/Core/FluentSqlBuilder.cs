namespace KISS.QueryBuilder.Core;

/// <summary>
///     A class that defines the fluent SQL builder type.
///     The core <see cref="FluentSqlBuilder{TRecordset}" /> partial class.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public partial class FluentSqlBuilder<TRecordset>
{
    /// <inheritdoc/>
    public string Sql
        => SqlBuilder.ToString();

    /// <inheritdoc />
    public DynamicParameters Parameters
        => SqlFormat.Parameters;

    private StringBuilder SqlBuilder { get; } = new();

    private SqlFormatter SqlFormat { get; } = new();

    private Type RootTable { get; } = typeof(TRecordset);

    private Dictionary<Type, string> TableAliasesMap { get; } = new()
    {
        [typeof(TRecordset)] = $"{ClauseConstants.DefaultTableAlias}{0}"
    };

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
}
