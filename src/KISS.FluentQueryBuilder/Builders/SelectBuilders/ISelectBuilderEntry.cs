namespace KISS.FluentQueryBuilder.Builders.SelectBuilders;

public interface ISelectBuilderEntry
{
    ISelectBuilder Select(LambdaExpression expression);
}
