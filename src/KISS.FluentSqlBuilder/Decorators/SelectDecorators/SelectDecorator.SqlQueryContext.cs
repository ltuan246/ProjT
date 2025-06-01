namespace KISS.FluentSqlBuilder.Decorators.SelectDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public sealed partial record SelectDecorator
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

            return SqlBuilder.ToString();
        }
    }
}
