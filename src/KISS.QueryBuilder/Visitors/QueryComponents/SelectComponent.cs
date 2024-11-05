namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>SELECT</c> clause.
/// </summary>
internal sealed class SelectComponent : QueryComponent, IQueryComponent
{
    /// <summary>
    ///     The table columns.
    /// </summary>
    public List<Expression> Selectors { get; } = [];

    /// <inheritdoc cref="QueryComponent.SqlBuilder" />
    public override StringBuilder SqlBuilder { get; } = new();

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        foreach (var selector in Selectors)
        {
            Translate(selector);
        }

        visitor.Visit(this);
    }

    /// <inheritdoc />
    protected override void Translate(MemberExpression memberExpression)
    {
    }
}
