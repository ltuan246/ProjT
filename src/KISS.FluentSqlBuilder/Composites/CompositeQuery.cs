namespace KISS.FluentSqlBuilder.Composites;

/// <summary>
///     A class that defines the fluent SQL builder type.
/// </summary>
public sealed partial class CompositeQuery
{
    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> SelectComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> SelectFromComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<(Type Recordset, Expression LeftKeySelector, Expression RightKeySelector)> JoinComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> WhereComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> GroupByComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> HavingComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> OrderByComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> LimitComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> OffsetComponents { get; } = [];

    /// <summary>
    ///     Gets the query components.
    /// </summary>
    public void SetQueries()
    {
        SetSelect();
        SetFrom();
        SetJoin();
        SetWhere();
        SetGroupBy();
        SetHaving();
        SetOrderBy();
        SetLimit();
        SetOffset();
    }
}
