namespace KISS.FluentSqlBuilder.Decorators.GroupByDecorators;

/// <summary>
///     Provides the expression tree logic for grouping and aggregating entities in the GroupByDecorator.
///     This class builds the LINQ expression block that performs grouping, aggregation, and output
///     transformation for SQL GROUP BY operations, using dynamically constructed expression variables.
/// </summary>
public sealed partial record GroupByDecorator
{
    /// <inheritdoc />
    public override string Sql
    {
        get
        {
            // Build the SQL query using CTE and GROUP BY logic.
            Append("WITH CommonTableExpression AS (");
            AppendLine(Inner.Sql);
            AppendLine(")");

            new EnumeratorProcessor<string>(SqlStatements[SqlStatement.GroupBy])
                .AccessFirst(_ =>
                {
                    StringBuilder outerSelectBuilder = new(),
                        innerSelectBuilder = new(),
                        groupByFilteringBuilder = new(),
                        onClauseBuilder = new();

                    // Build SELECT clause for aggregation keys.
                    new EnumeratorProcessor<KeyValuePair<string, Type>>(AggregationKeys)
                        .AccessFirst(kv =>
                        {
                            outerSelectBuilder.Append($"GP.{kv.Key}");
                        })
                        .AccessRemaining(kv =>
                        {
                            outerSelectBuilder.AppendLine($", GP.{kv.Key}");
                        })
                        .AccessLast(() => outerSelectBuilder.Append(','))
                        .Execute();

                    // Build SELECT, GROUP BY, and ON clauses for grouping keys.
                    new EnumeratorProcessor<KeyValuePair<string, Type>>(GroupingKeys)
                        .AccessFirst(kv =>
                        {
                            outerSelectBuilder.Append($"GP.{kv.Key}");
                            innerSelectBuilder.Append($"{kv.Key}");
                            groupByFilteringBuilder.Append($"GROUP BY {kv.Key}");
                            onClauseBuilder.Append($"CTE.{kv.Key} = GP.{kv.Key}");
                        })
                        .AccessRemaining(kv =>
                        {
                            outerSelectBuilder.AppendLine($", GP.{kv.Key}");
                            innerSelectBuilder.AppendLine($", {kv.Key}");
                            groupByFilteringBuilder.AppendLine($", {kv.Key}");
                            onClauseBuilder.AppendLine(
                                $"AND CTE.{kv.Key} = GP.{kv.Key}");
                        })
                        .AccessLast(() => outerSelectBuilder.Append(','))
                        .Execute();

                    outerSelectBuilder.Append("CTE.*");

                    // Build SELECT clause for aggregate functions.
                    new EnumeratorProcessor<string>(SqlStatements[SqlStatement.SelectAggregate])
                        .AccessFirst(fs =>
                        {
                            if (innerSelectBuilder.Length > 0)
                            {
                                innerSelectBuilder.Append(',');
                            }

                            innerSelectBuilder.Append($" {fs}");
                        })
                        .AccessRemaining(fs =>
                        {
                            innerSelectBuilder.AppendLine($", {fs}");
                        })
                        .Execute();

                    // Build HAVING clause for group filtering.
                    new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Having])
                        .AccessFirst(fs =>
                        {
                            groupByFilteringBuilder.Append($" HAVING {fs}");
                        })
                        .AccessRemaining(fs =>
                        {
                            groupByFilteringBuilder.AppendLine($"AND {fs}");
                        })
                        .Execute();

                    // Compose the final SQL query for grouped results.
                    Append($@"
                        SELECT
                            {outerSelectBuilder}
                        FROM CommonTableExpression CTE
                        JOIN (
                            SELECT
                                {innerSelectBuilder}
                            FROM CommonTableExpression
                                {groupByFilteringBuilder}
                        ) GP
                        ON {onClauseBuilder};
                    ");

                    AppendLine();
                })
                .Execute();

            return SqlBuilder.ToString();
        }
    }
}
