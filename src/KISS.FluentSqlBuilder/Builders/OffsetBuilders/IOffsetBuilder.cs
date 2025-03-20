namespace KISS.FluentSqlBuilder.Builders.OffsetBuilders;

/// <summary>
///     A marker interface for building <c>OFFSET</c> clauses. This interface serves
///     as the base for all offset builders in the query building process.
/// </summary>
public interface IOffsetBuilder : ISqlBuilder;

/// <summary>
///     An interface for building <c>OFFSET</c> clauses with support for pagination
///     in single-table queries. This interface enables skipping a specified number
///     of rows in the result set.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view being queried.</typeparam>
/// <typeparam name="TReturn">The type of the result set after applying the offset.</typeparam>
public interface IOffsetBuilder<TRecordset, TReturn>
   : ISqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends an <c>OFFSET</c> clause to skip a specified number of rows in
    ///     the result set. This method is typically used with ORDER BY and FETCH
    ///     clauses for implementing pagination.
    /// </summary>
    /// <param name="offset">
    ///     The number of rows to skip before starting to return rows.
    ///     For example, an offset of 20 will skip the first 20 rows.
    /// </param>
    /// <returns>
    ///     An <see cref="ISqlBuilder{TRecordset, TReturn}" /> instance that can be used
    ///     to add a FETCH clause or execute the query.
    /// </returns>
    ISqlBuilder<TRecordset, TReturn> Offset(int offset);
}

/// <summary>
///     An interface for building <c>OFFSET</c> clauses with support for pagination
///     in two-table queries. This interface enables skipping a specified number
///     of rows in joined result sets.
/// </summary>
/// <typeparam name="TFirst">The type representing the first table in the join chain.</typeparam>
/// <typeparam name="TSecond">The type representing the second table in the join chain.</typeparam>
/// <typeparam name="TReturn">The type of the result set after applying the offset.</typeparam>
public interface IOffsetBuilder<TFirst, TSecond, TReturn>
   : ISqlBuilder<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends an <c>OFFSET</c> clause to skip a specified number of rows in
    ///     the joined result set. This method is typically used with ORDER BY and
    ///     FETCH clauses for implementing pagination in multi-table queries.
    /// </summary>
    /// <param name="offset">
    ///     The number of rows to skip before starting to return rows.
    ///     For example, an offset of 20 will skip the first 20 rows.
    /// </param>
    /// <returns>
    ///     An <see cref="ISqlBuilder{TFirst, TSecond, TReturn}" /> instance that can be used
    ///     to add a FETCH clause or execute the query.
    /// </returns>
    ISqlBuilder<TFirst, TSecond, TReturn> Offset(int offset);
}

/// <summary>
///     An interface for building <c>OFFSET</c> clauses with support for pagination
///     in three-table queries. This interface enables skipping a specified number
///     of rows in complex joined result sets.
/// </summary>
/// <typeparam name="TFirst">The type representing the first table in the join chain.</typeparam>
/// <typeparam name="TSecond">The type representing the second table in the join chain.</typeparam>
/// <typeparam name="TThird">The type representing the third table in the join chain.</typeparam>
/// <typeparam name="TReturn">The type of the result set after applying the offset.</typeparam>
public interface IOffsetBuilder<TFirst, TSecond, TThird, TReturn>
   : ISqlBuilder<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Appends an <c>OFFSET</c> clause to skip a specified number of rows in
    ///     the complex joined result set. This method is typically used with ORDER BY
    ///     and FETCH clauses for implementing pagination in multi-table queries.
    /// </summary>
    /// <param name="offset">
    ///     The number of rows to skip before starting to return rows.
    ///     For example, an offset of 20 will skip the first 20 rows.
    /// </param>
    /// <returns>
    ///     An <see cref="ISqlBuilder{TFirst, TSecond, TThird, TReturn}" /> instance that can be used
    ///     to add a FETCH clause or execute the query.
    /// </returns>
    ISqlBuilder<TFirst, TSecond, TThird, TReturn> Offset(int offset);
}

/// <summary>
///     An interface for building <c>OFFSET</c> clauses with support for pagination
///     in grouped queries. This interface enables skipping a specified number of
///     rows in grouped result sets.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view being queried.</typeparam>
/// <typeparam name="TReturn">The type of the result set after grouping and applying the offset.</typeparam>
public interface IGroupOffsetBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends an <c>OFFSET</c> clause to skip a specified number of rows in
    ///     the grouped result set. This method is typically used with ORDER BY and
    ///     FETCH clauses for implementing pagination in grouped queries.
    /// </summary>
    /// <param name="offset">
    ///     The number of groups to skip before starting to return groups.
    ///     For example, an offset of 10 will skip the first 10 groups.
    /// </param>
    /// <returns>
    ///     An <see cref="IGroupSqlBuilder{TRecordset, TReturn}" /> instance that can be used
    ///     to add a FETCH clause or execute the query.
    /// </returns>
    IGroupSqlBuilder<TRecordset, TReturn> Offset(int offset);
}
