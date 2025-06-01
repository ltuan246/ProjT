namespace KISS.FluentSqlBuilder.Builders.HavingBuilders;

/// <summary>
///     A marker interface for building <c>HAVING</c> clauses. This interface serves
///     as the base for all having builders in the query building process.
/// </summary>
public interface IHavingBuilder;

/// <summary>
///     An interface for building <c>HAVING</c> clauses with support for filtering
///     grouped results based on aggregate conditions. This interface enables the
///     construction of complex filters on grouped data.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view being queried.</typeparam>
/// <typeparam name="TReturn">The type of the combined result set after grouping and filtering.</typeparam>
public interface IHavingBuilder<TRecordset, TReturn> :
    IGroupSelectBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends a <c>HAVING</c> clause to filter grouped results based on aggregate
    ///     conditions. If a HAVING clause already exists, appends an AND clause to
    ///     combine the conditions.
    /// </summary>
    /// <param name="condition">
    ///     An expression that defines the filtering condition using aggregate functions.
    ///     The expression takes an <see cref="AggregationBuilder{TRecordset}"/> that
    ///     provides access to aggregate functions (COUNT, SUM, AVG, etc.).
    /// </param>
    /// <returns>
    ///     The <see cref="IHavingBuilder{TRecordset, TReturn}" /> instance, enabling
    ///     further having conditions or group select operations.
    /// </returns>
    IHavingBuilder<TRecordset, TReturn> Having(Expression<Func<AggregationBuilder<TRecordset>, bool>> condition);
}
