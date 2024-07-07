namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder
{
    public string Operation(params IComponent[] components)
    {
        QueryBuilder visitor = new();

        foreach (IComponent component in components)
        {
            component.Accept(visitor);
        }

        StringBuilder stringBuilder = new();

        foreach (var item in visitor.Builder)
        {
            stringBuilder.Append(item);
        }

        return stringBuilder.ToString();
    }
}