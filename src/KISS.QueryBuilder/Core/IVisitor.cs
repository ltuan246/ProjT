namespace KISS.QueryBuilder.Core;

public interface IVisitor
{
    void Visit(IComponent concreteComponent);

    void Visit<TComponent, TField>(ComparisonOperatorFilterDefinition<TComponent, TField> operatorFilterDefinition);

    void Visit<TComponent, TField>(SingleItemAsArrayOperatorFilterDefinition<TComponent, TField> operatorFilterDefinition);

    void Visit(LogicalOperatorFieldDefinition logicalOperatorFieldDefinition);
}