namespace KISS.QueryPredicateBuilder.Builders.FetchBuilders;

/// <summary>
///     Defines the fetch builder type.
/// </summary>
public sealed record FetchBuilder
{
    // /// <summary>
    // /// Appends the FETCH NEXT clause, the <paramref name="rows"/>, and the ROWS ONLY clause to the builder.
    // /// </summary>
    // /// <param name="rows">The number of rows to fetch.</param>
    // /// <returns>The FETCH NEXT clause.</returns>
    // public FetchDefinition FetchNext(int rows)
    //     => new($"FETCH NEXT {rows:raw}");

    /// <summary>
    ///     Appends the LIMIT clause and the <paramref name="rows" /> to the builder.
    /// </summary>
    /// <param name="rows">The number of rows to fetch.</param>
    /// <returns>The LIMIT clause.</returns>
    public FetchDefinition Limit(int rows)
        => new($"LIMIT {rows:raw}");

    /// <summary>
    ///     Appends the OFFSET clause and the <paramref name="offset" /> to the builder.
    /// </summary>
    /// <param name="offset">The number of rows to skip.</param>
    /// <returns>The OFFSET clause.</returns>
    public OffsetDefinition Offset(int offset)
        => new($"OFFSET {offset:raw}");
}
