namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder
{
    private void Join(string separator, IEnumerable<IComponent> expressions)
    {
        using IEnumerator<IComponent> enumerator = expressions.GetEnumerator();
        if (enumerator.MoveNext())
        {
            enumerator.Current.Accept(this);
            while (enumerator.MoveNext())
            {
                Builder.Append(separator);
                enumerator.Current.Accept(this);
            }
        }
    }
}