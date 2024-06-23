namespace KISS.QueryBuilder.Queries.Projections;

public sealed record SingleFieldProjectionDefinition((RenderedFieldDefinition field, bool isIncluding) FieldDefinition)
    : ISingleFieldProjectionDefinition
{
    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);
}