namespace KISS.FluentSqlBuilder.Decorators.OrderByDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
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

            return SqlBuilder.ToString();
        }
    }
}