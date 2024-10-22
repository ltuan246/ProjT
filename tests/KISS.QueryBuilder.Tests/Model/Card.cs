namespace KISS.QueryBuilder.Tests.Model;

public class Card
{
    [Name("id")] public required string Id { get; set; }
    [Name("playerClass")] public required string PlayerClass { get; set; }
    [Name("type")] public required string Type { get; set; }
    [Name("name")] public required string Name { get; set; }
    [Name("cost")] public int? Cost { get; set; }
    [Ignore, NotMapped] public DustCost? DustCost { get; set; }
}
