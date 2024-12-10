namespace KISS.QueryBuilder.Tests.Model;

public class CardFlat
{
    [Name("id")] public required string Id { get; set; }
    [Name("player_class")] public required string PlayerClass { get; set; }
    [Name("type")] public required string Type { get; set; }
    [Name("name")] public required string Name { get; set; }
}
