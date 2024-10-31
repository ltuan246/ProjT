namespace KISS.QueryBuilder.Builders.GroupByBuilders;

/// <summary>
///     An interface for adding a GROUP BY clause to the query.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface IGroupByBuilderEntry<TRecordset> : IFluentSqlBuilder<TRecordset>
{
    /// <summary>
    ///     Appends a <c>GROUP BY</c> clause to the query, grouping records by a specified key.
    /// </summary>
    /// <param name="keySelector">The selector for the column to use as the group key.</param>
    /// <param name="mapSelector">
    ///     An expression to map the results of each group into the desired output format.
    ///     If a <c>SELECT</c> clause is present, it will be replaced.
    /// </param>
    /// <typeparam name="TKey">The type of the key to group by, which must be comparable.</typeparam>
    /// <typeparam name="TMap">The results of the grouped tables to be mapped back into the selector.</typeparam>
    /// <returns>The <see cref="IGroupByBuilder{TRecordset}" /> instance for further query construction.</returns>
    IGroupByBuilder<TMap> GroupBy<TKey, TMap>(
        Expression<Func<TRecordset, TKey>> keySelector,
        Expression<Func<TKey, List<TRecordset>, TMap>> mapSelector)
        where TKey : IComparable<TKey>
        where TMap : IEntityBuilder;
}
