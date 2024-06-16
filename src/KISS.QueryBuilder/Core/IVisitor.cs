namespace KISS.QueryBuilder.Core;

public interface IVisitor
{
    void Visit(IQuerying concreteQuerying);

    void Visit(IFilterDefinition filterDefinition);

    void Visit<TEntity, TField>(SingleItemAsArrayOperatorFilterDefinition<TEntity, TField> operatorFilterDefinition);

    void Visit(AndFilterDefinition logicalOperatorFieldDefinition);

    void Visit(OrFilterDefinition logicalOperatorFieldDefinition);
}