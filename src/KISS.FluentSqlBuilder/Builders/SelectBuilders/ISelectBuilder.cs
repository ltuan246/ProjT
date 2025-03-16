namespace KISS.FluentSqlBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface for building <c>SELECT</c> queries.
/// </summary>
public interface ISelectBuilder;

/// <summary>
///     An interface for building <c>SELECT</c> queries.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISelectBuilder<TRecordset, TReturn> :
    IOrderByBuilderEntry<TRecordset, TReturn>;

/// <summary>
///     An interface for building <c>SELECT</c> queries.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISelectBuilder<TFirst, TSecond, TReturn> :
    IOrderByBuilderEntry<TFirst, TSecond, TReturn>;

/// <summary>
///     An interface for building <c>SELECT</c> queries.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISelectBuilder<TFirst, TSecond, TThird, TReturn> :
    IOrderByBuilderEntry<TFirst, TSecond, TThird, TReturn>;

/// <summary>
///     An interface for building <c>SELECT</c> queries.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IGroupSelectBuilder<TRecordset, TReturn> :
    IGroupOrderByBuilderEntry<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>SELECT</c> clause to the builder.
    /// </summary>
    /// <param name="aggregationType">The aggregation types used in queries for calculating summary statistics.</param>
    /// <param name="selector">The table columns.</param>
    /// <param name="alias">The alias columns.</param>
    /// <returns>The <see cref="ISelectAggregateBuilder{TRecordset, TReturn}" /> instance.</returns>
    IGroupSelectBuilder<TRecordset, TReturn> SelectAggregate(
        SqlAggregation aggregationType,
        Expression<Func<TRecordset, IComparable>> selector,
        string alias);
}
