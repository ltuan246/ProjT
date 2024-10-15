namespace KISS.QueryPredicateBuilder.Core;

/// <summary>
///     Implements the Composite, which is a structural design pattern that lets compose objects into tree structures.
/// </summary>
public sealed partial class QueryBuilder
{
    /// <summary>
    ///     The Composite executes its primary logic in a particular way. It
    ///     traverses recursively through all its children, collecting and
    ///     summing their results. Since the composite's children pass these
    ///     calls to their children and so forth, the whole object tree is
    ///     traversed as a result.
    /// </summary>
    /// <param name="components">The Query Builders.</param>
    /// <typeparam name="TEntity">The type of the record.</typeparam>
    /// <returns>Returns the SQL query and the parameter values are put into Dapper's DynamicParameters collection.</returns>
    public (string Sql, DynamicParameters Parameters) Operation<TEntity>([NotNull] params IComponent[] components)
    {
        QueryBuilder visitor = new();

        foreach (var component in components)
        {
            component.Accept(visitor);
        }

        StringBuilder sqlBuilder = new();

        if (visitor.Builder.TryGetValue(ClauseAction.Select, out var select))
        {
            sqlBuilder.Append(select);
            sqlBuilder.Append(Constants.Space);
        }
        else
        {
            var entity = typeof(TEntity);
            var table = entity.Name;
            var propsName = entity.GetProperties().Select(p => $"[{p.Name}]").ToArray();
            var columns = string.Join(", ", propsName);
            const string sqlSelectClause = "SELECT {0} FROM {1}s ";
            sqlBuilder.AppendFormat(sqlSelectClause, columns, table);
        }

        if (visitor.Builder.TryGetValue(ClauseAction.Where, out var where))
        {
            sqlBuilder.Append(ClauseAction.Where);
            sqlBuilder.Append(Constants.Space);
            sqlBuilder.Append(where);
            sqlBuilder.Append(Constants.Space);
        }

        if (visitor.Builder.TryGetValue(ClauseAction.OrderBy, out var sort))
        {
            sqlBuilder.Append(sort);
            sqlBuilder.Append(Constants.Space);
        }

        if (visitor.Builder.TryGetValue(ClauseAction.FetchNext, out var fetchNext))
        {
            sqlBuilder.Append(fetchNext);
            sqlBuilder.Append(Constants.Space);
        }

        if (visitor.Builder.TryGetValue(ClauseAction.Offset, out var offset))
        {
            sqlBuilder.Append(offset);
            sqlBuilder.Append(Constants.Space);
        }

        return (sqlBuilder.ToString(), visitor.Formatter.Parameters);
    }
}
