namespace KISS.QueryBuilder.Interfaces;

public interface IOperatorFilterDefinition : IQuerying
{
    (ComparisonOperator comparisonOperator, string fieldName, object value) QueryParameter { get; }
}