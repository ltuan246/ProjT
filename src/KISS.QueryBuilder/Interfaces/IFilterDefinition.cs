namespace KISS.QueryBuilder.Interfaces;

public interface IFilterDefinition : IQuerying
{
    (ComparisonOperator comparisonOperator, string fieldName, object value) QueryParameter { get; }
}