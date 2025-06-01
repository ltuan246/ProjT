namespace KISS.FluentSqlBuilder.Decorators.JoinDecorators;

/// <summary>
///     Provides the SQL query construction logic for the <see cref="JoinDecorator"/>.
///     This class assembles the SELECT, FROM, and JOIN clauses for SQL queries that
///     involve joining multiple tables or entities, using the configured statement lists
///     and type-safe alias mapping.
/// </summary>
public sealed partial record JoinDecorator
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

            // Build the JOIN clauses from the configured join statements.
            new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Join])
                .AccessFirst(fs =>
                {
                    AppendLine($"{fs}");
                })
                .AccessRemaining(fs =>
                {
                    AppendLine($"{fs}");
                })
                .AccessLast(() => AppendLine())
                .Execute();

            // Return the complete SQL query as a string.
            return SqlBuilder.ToString();
        }
    }
}
