namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder : IVisitor
{
    public void Visit(OperatorFilterDefinition element)
        => AppendFormattable(element.Formattable);

    public void Visit(SingleItemAsArrayOperatorFilterDefinition element)
        => AppendFormattable(element.Formattable);

    public void Visit(RangeFilterDefinition element)
        => AppendFormattable(element.Formattable);

    public void Visit(CombinedFilterDefinition element)
    {
        PushState(element.Clause, element.Components.Length);
        Join(element.Separator, element.Components);
        PopState();
    }
}