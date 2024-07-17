namespace KISS.QueryPredicateBuilder.Builders.FetchBuilders;

public sealed record FetchBuilder
{
    /// <summary>
    /// Appends the FETCH NEXT clause, the <paramref name="rows"/>, and the ROWS ONLY clause to the builder.
    /// </summary>
    /// <param name="rows">The number of rows to fetch.</param>
    /// <returns>The FETCH NEXT clause</returns>
    public FetchDefinition FetchNext(int rows)
        => new($"FETCH NEXT {rows:raw}");

    public FetchDefinition Limit(int rows)
        => new($"LIMIT {rows:raw}");
}