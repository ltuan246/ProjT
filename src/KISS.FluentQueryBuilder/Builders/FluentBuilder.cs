namespace KISS.FluentQueryBuilder.Builders;

public sealed partial class FluentBuilder : IFluentBuilder, IFluentBuilderEntry
{
    public ISelectBuilder Select([NotNull] LambdaExpression expression)
    {
        Visit(expression.Body);
        return this;
    }

    public ISelectDistinctBuilder SelectDistinct() => this;

    public IWhereBuilder Where() => this;
}
