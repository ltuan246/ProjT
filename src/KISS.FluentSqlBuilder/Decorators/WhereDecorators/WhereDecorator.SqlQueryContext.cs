namespace KISS.FluentSqlBuilder.Decorators.WhereDecorators;

/// <summary>
///     Provides the SQL query construction logic for the <see cref="WhereDecorator"/>.
///     This class assembles the WHERE clause for SQL queries, supporting the addition of
///     filtering conditions to composite SQL queries with type-safe result processing.
/// </summary>
public sealed partial record WhereDecorator
{
    /// <inheritdoc />
    public override string Sql
    {
        get
        {
            SqlBuilder.Clear();
            Append(Inner.Sql);

            // Build the WHERE clause from the configured where statements.
            new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Where])
                .AccessFirst(fs =>
                {
                    Append("WHERE");
                    AppendLine($"{fs}");
                })
                .AccessRemaining(fs =>
                {
                    AppendLine($"AND {fs}");
                })
                .AccessLast(() => AppendLine())
                .Execute();

            // Return the complete SQL query as a string.
            return SqlBuilder.ToString();
        }
    }
}
