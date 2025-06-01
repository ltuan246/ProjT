namespace KISS.FluentSqlBuilder.Decorators.SelectDecorators;

/// <summary>
///     Provides the SQL query construction logic for the <see cref="SelectDecorator"/>.
///     This class assembles the SELECT and FROM clauses for SQL queries, supporting
///     both simple and complex query scenarios with type-safe result processing.
/// </summary>
public sealed partial record SelectDecorator
{
    /// <inheritdoc />
    public override string Sql
    {
        get
        {
            SqlBuilder.Clear();

            // Build the SELECT clause from the configured select statements.
            new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Select])
                .AccessFirst(fs =>
                {
                    Append("SELECT");
                    AppendLine($"{fs}");
                })
                .AccessRemaining(fs =>
                {
                    AppendLine($", {fs}");
                })
                .AccessLast(() => AppendLine())
                .Execute();

            // Add the FROM clause with the table name and alias.
            var alias = GetAliasMapping(InEntityType);
            Append("FROM");
            AppendLine($"{TypeUtils.GetTableName(InEntityType)} AS {alias}");
            AppendLine();

            // Return the complete SQL query as a string.
            return SqlBuilder.ToString();
        }
    }
}
