namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder
{
    public (string Sql, DynamicParameters Parameters) Operation<TEntity>(params IComponent[] components)
    {
        QueryBuilder visitor = new();

        foreach (IComponent component in components)
        {
            component.Accept(visitor);
        }

        StringBuilder sqlBuilder = new();

        Type entity = typeof(TEntity);
        string table = entity.Name;
        string[] propsName = entity.GetProperties().Select(p => $"[{p.Name}]").ToArray();
        string columns = string.Join(", ", propsName);
        const string sqlSelectClause = "SELECT {0} FROM {1}s ";
        sqlBuilder.AppendFormat(sqlSelectClause, columns, table);

        foreach (var item in visitor.Builder)
        {
            sqlBuilder.Append(item.Key);
            sqlBuilder.Append(Constants.Space);
            sqlBuilder.Append(item.Value);
        }

        return (sqlBuilder.ToString(), visitor.Formatter.Parameters);
    }
}