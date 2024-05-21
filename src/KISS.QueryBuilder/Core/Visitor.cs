namespace KISS.QueryBuilder.Core;

public abstract class Visitor : IVisitor
{
    public void Visit(IComponent concreteComponent) => concreteComponent.Accept(this);

    public virtual void Visit<TComponent, TField>(ComparisonOperatorFilterDefinition<TComponent, TField> operatorFilterDefinition) { }

    public virtual void Visit<TComponent, TField>(SingleItemAsArrayOperatorFilterDefinition<TComponent, TField> operatorFilterDefinition) { }

    public virtual void Visit(LogicalOperatorFieldDefinition logicalOperatorFieldDefinition) { }
}