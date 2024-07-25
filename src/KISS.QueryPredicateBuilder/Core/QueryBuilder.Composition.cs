namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder
{
    public (string Sql, DynamicParameters Parameters) Operation<TEntity>([NotNull] params IComponent[] components)
    {
        QueryBuilder visitor = new();

        foreach (IComponent component in components)
        {
            component.Accept(visitor);
        }

        StringBuilder sqlBuilder = new();

        if (visitor.Builder.TryGetValue(ClauseAction.Select, out StringBuilder? select))
        {
            sqlBuilder.Append(select);
            sqlBuilder.Append(Constants.Space);
        }
        else
        {
            Type entity = typeof(TEntity);
            string table = entity.Name;
            string[] propsName = entity.GetProperties().Select(p => $"[{p.Name}]").ToArray();
            string columns = string.Join(", ", propsName);
            const string sqlSelectClause = "SELECT {0} FROM {1}s ";
            sqlBuilder.AppendFormat(sqlSelectClause, columns, table);
        }

        if (visitor.Builder.TryGetValue(ClauseAction.Where, out StringBuilder? where))
        {
            sqlBuilder.Append(ClauseAction.Where);
            sqlBuilder.Append(Constants.Space);
            sqlBuilder.Append(where);
            sqlBuilder.Append(Constants.Space);
        }

        if (visitor.Builder.TryGetValue(ClauseAction.OrderBy, out StringBuilder? sort))
        {
            sqlBuilder.Append(sort);
            sqlBuilder.Append(Constants.Space);
        }

        if (visitor.Builder.TryGetValue(ClauseAction.FetchNext, out StringBuilder? fetchNext))
        {
            sqlBuilder.Append(fetchNext);
            sqlBuilder.Append(Constants.Space);
        }

        if (visitor.Builder.TryGetValue(ClauseAction.Offset, out StringBuilder? offset))
        {
            sqlBuilder.Append(offset);
            sqlBuilder.Append(Constants.Space);
        }

        return (sqlBuilder.ToString(), visitor.Formatter.Parameters);
    }
}