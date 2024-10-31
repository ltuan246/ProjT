namespace KISS.QueryBuilder.Tests.Model;

public class DustCost : IEntityBuilder
{
    [Name("id")] public required string Id { get; set; }
    [Name("card_id")] public required string CardId { get; set; }
    [Name("action")] public required string Action { get; set; }
    [Name("cost")] public int Cost { get; set; }
}
