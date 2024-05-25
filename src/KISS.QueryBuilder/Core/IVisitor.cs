namespace KISS.QueryBuilder.Core;

public interface IVisitor
{
    void Visit(IComponent concreteComponent);

    void Visit<TComponent, TField>(OperatorFilterDefinition<TComponent, TField> operatorFilterDefinition);

    void Visit<TComponent, TField>(SingleItemAsArrayOperatorFilterDefinition<TComponent, TField> operatorFilterDefinition);

    void Visit(AndFilterDefinition logicalOperatorFieldDefinition);

    void Visit(OrFilterDefinition logicalOperatorFieldDefinition);
}