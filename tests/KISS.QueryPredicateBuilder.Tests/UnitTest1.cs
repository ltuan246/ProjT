namespace KISS.QueryPredicateBuilder.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        IComponent[] components = [new ConcreteComponentA(), new ConcreteComponentB()];

        QueryBuilder visitor = new();
        var res = visitor.Operation(components);
    }
}