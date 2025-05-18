namespace KISS.FluentSqlBuilder.Decorators.WhereDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
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

            return SqlBuilder.ToString();
        }
    }
}