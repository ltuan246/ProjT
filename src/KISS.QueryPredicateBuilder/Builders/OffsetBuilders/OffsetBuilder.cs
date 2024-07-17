namespace KISS.QueryPredicateBuilder.Builders.OffsetBuilders;

public sealed record OffsetBuilder
{
    public OffsetDefinition Offset(int offset)
        => new($"OFFSET {offset:raw}");
}