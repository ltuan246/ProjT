namespace KISS.FluentSqlBuilder.Decorators.LimitDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TIn">The type representing the database record set.</typeparam>
/// <typeparam name="TOut">The combined type to return.</typeparam>
public sealed partial record LimitDecorator
{
    /// <inheritdoc />
    public override string Sql
    {
        get
        {
            SqlBuilder.Clear();
            Append(Inner.Sql);

            new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Limit])
                .AccessFirst(fs =>
                {
                    Append("LIMIT");
                    AppendLine($"{fs}");
                    AppendLine();
                })
                .Execute();

            return SqlBuilder.ToString();
        }
    }
}