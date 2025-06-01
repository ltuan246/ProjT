namespace KISS.FluentSqlBuilder.Decorators.OrderByDecorators;

/// <summary>
///     Provides the SQL query construction logic for the <see cref="OrderByDecorator"/>.
///     This class assembles the ORDER BY clause for SQL queries, supporting the addition of
///     sorting logic to composite SQL queries with type-safe result processing.
/// </summary>
public sealed partial record OrderByDecorator
{
    /// <inheritdoc />
    public override string Sql
    {
        get
        {
            SqlBuilder.Clear();
            Append(Inner.Sql);

            // Build the ORDER BY clause from the configured order by statements.
            new EnumeratorProcessor<string>(SqlStatements[SqlStatement.OrderBy])
                .AccessFirst(fs =>
                {
                    Append("ORDER BY");
                    AppendLine($"{fs}");
                })
                .AccessRemaining(fs =>
                {
                    AppendLine($", {fs}");
                })
                .AccessLast(() => AppendLine())
                .Execute();

            // Return the complete SQL query as a string.
            return SqlBuilder.ToString();
        }
    }
}
