namespace KISS.FluentQueryBuilder.Builders;

/// <summary>
///     A class that defines the fluent SQL builder type. The core <see cref="FluentBuilder{TEntity}" /> partial class.
/// </summary>
/// <param name="DbConnection">The connection to a database.</param>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public sealed partial record FluentBuilder<TEntity>(DbConnection DbConnection)
    : IFluentBuilder<TEntity>, IFluentBuilderEntry<TEntity>
{
    /// <inheritdoc />
    public ISelectBuilder<TEntity> Select<TResult>([NotNull] Expression<Func<TEntity, TResult>> expression)
    {
        SetEntryClause(ClauseAction.Select, expression.Body);

        Append(" SELECT ");
        Translate(expression.Body);
        Append($" FROM {typeof(TEntity).Name}s ");

        return this;
    }

    /// <inheritdoc />
    public ISelectDistinctBuilder SelectDistinct([NotNull] LambdaExpression expression)
    {
        SetEntryClause(ClauseAction.SelectDistinct, expression.Body);

        Append(" SELECT DISTINCT ");
        Translate(expression.Body);
        Append($" FROM {typeof(TEntity).Name}s ");

        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TEntity> Where([NotNull] Expression<Func<TEntity, bool>> expression)
    {
        if (!EntryClause.ContainsKey(ClauseAction.Select) && !EntryClause.ContainsKey(ClauseAction.SelectDistinct))
        {
            var entity = typeof(TEntity);
            var table = entity.Name;
            var propsName = entity.GetProperties().Select(p => $"[{p.Name}]").ToArray();
            var columns = string.Join(", ", propsName);
            Append($"SELECT {columns} FROM {table}s ");
        }

        SetEntryClause(ClauseAction.Where, expression.Body);

        Append(" WHERE ");
        Translate(expression.Body);

        return this;
    }

    /// <inheritdoc />
    public IList<TEntity> ToList()
        => DbConnection.Query<TEntity>(Sql, Parameters).ToList();
}
