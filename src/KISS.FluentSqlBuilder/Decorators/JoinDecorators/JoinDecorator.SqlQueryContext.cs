namespace KISS.FluentSqlBuilder.Decorators.JoinDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public sealed partial record JoinDecorator
{
    /// <inheritdoc />
    public override string Sql
    {
        get
        {
            SqlBuilder.Clear();

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

            var alias = GetAliasMapping(InEntityType);
            Append("FROM");
            AppendLine($"{TypeUtils.GetTableName(InEntityType)} AS {alias}");
            AppendLine();

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

            return SqlBuilder.ToString();
        }
    }
}