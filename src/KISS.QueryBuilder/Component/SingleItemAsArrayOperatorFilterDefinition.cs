namespace KISS.QueryBuilder.Component;

public sealed record SingleItemAsArrayOperatorFilterDefinition<TEntity, TField>(
    SingleItemAsArrayOperator Operator,
    ExpressionFieldDefinition<TEntity, TField> Field,
    params TField[] Values) : IQuerying
{
    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}