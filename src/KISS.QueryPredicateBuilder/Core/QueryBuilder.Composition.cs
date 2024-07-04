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

        return visitor.Builder.ToString();
    }
}