namespace KISS.QueryBuilder.Interfaces;

public interface ISingleItemAsArrayOperatorFilterDefinition : IQuerying
{
    (SingleItemAsArrayOperator singleItemAsArrayOperator, string fieldName, object[] values) QueryParameter { get; }
}