namespace KISS.QueryBuilder.Tests.Model;

public class ComponentTest
{
    public string AsString { get; } = "";
    public int AsInt { get; }
    public bool AsBoolean { get; }
    public Guid AsGuid { get; } = Guid.Empty;
    public DateTime AsDateTime { get; } = DateTime.Now;
    public ComponentTest AsNestedComponent { get; } = new();
}