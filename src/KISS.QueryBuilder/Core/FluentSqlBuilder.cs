namespace KISS.QueryBuilder.Core;

/// <summary>
///     A class that defines the fluent SQL builder type. The core <see cref="FluentSqlBuilder{TEntity}" /> partial class.
/// </summary>
/// <typeparam name="TEntity">The type of results to return.</typeparam>
public sealed partial class FluentSqlBuilder<TEntity>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FluentSqlBuilder{TEntity}" /> class.
    /// </summary>
    public FluentSqlBuilder()
    {
        SqlBuilder = new StringBuilder();

        var entity = typeof(TEntity);
        var table = entity.Name;

        Append($"SELECT {TemporaryColumnsTemplate} FROM {table}s {DefaultEntityAliasTemplate}");
    }

    private StringBuilder SqlBuilder { get; }

    private SqlFormatter SqlFormat { get; } = new();

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    private bool HasOpenParentheses { get; set; }

    private void OpenParentheses()
    {
        HasOpenParentheses = true;
        SqlBuilder.Append(OpenParenthesis);
    }

    private void CloseParentheses()
    {
        if (!HasOpenParentheses)
        {
            return;
        }

        HasOpenParentheses = false;
        SqlBuilder.Append(CloseParenthesis);
    }

    private void AddCommaSeparated()
        => SqlBuilder.Append(Comma);
}
