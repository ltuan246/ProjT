namespace KISS.QueryBuilder.Utils;

internal sealed class StatesBuilder
{
    public required QueryingContext Context { get; init; }

    public required int Position { get; set; }

    public required int Length { get; init; }
}