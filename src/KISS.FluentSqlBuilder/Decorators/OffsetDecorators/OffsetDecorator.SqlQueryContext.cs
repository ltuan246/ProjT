namespace KISS.FluentSqlBuilder.Decorators.OffsetDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public sealed partial record OffsetDecorator
{
    /// <inheritdoc />
    public override string Sql
    {
        get
        {
            SqlBuilder.Clear();
            Append(Inner.Sql);

            new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Offset])
                .AccessFirst(fs =>
                {
                    Append("OFFSET");
                    AppendLine($"{fs}");
                    AppendLine();
                })
                .Execute();

            return SqlBuilder.ToString();
        }
    }
}