namespace KISS.FluentSqlBuilder.QueryChain.JoinHandlers;

/// <summary>
///     A handler for processing join operations in a query chain, linking two relations via key equality.
/// </summary>
/// <typeparam name="TRelation">The type of the relation (source table or entity).</typeparam>
/// <param name="LeftKeySelector">An expression selecting the key from the left relation for the join condition (e.g., left => left.Id).</param>
/// <param name="RightKeySelector">An expression selecting the key from the right relation for the join condition (e.g., right => right.Id).</param>
/// <param name="MapSelector">An expression mapping the joined result into the output type (e.g., left => left.RightRelation).</param>
public abstract partial record JoinHandler<TRelation>(
    Expression LeftKeySelector,
    Expression RightKeySelector,
    Expression MapSelector) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
    {
        var relationType = typeof(TRelation);
        var alias = Composite.GetAliasMapping(relationType);
        var sourceProperties = relationType.GetProperties()
            .Where(p => p.CanWrite)
            .Select(p => $"{alias}.{p.Name} AS {alias}_{p.Name}")
            .ToList();

        Composite.SqlStatements[SqlStatement.Select].Add($"{string.Join(", ", sourceProperties)}");

        Append("INNER JOIN");
        AppendLine($"{relationType.Name}s {alias}", true);
        AppendLine(" ON ", true);
        Translate(LeftKeySelector);
        Append(" = ");
        Translate(RightKeySelector);

        Composite.SqlStatements[SqlStatement.Join].Add($"{StatementBuilder}");
    }
}
