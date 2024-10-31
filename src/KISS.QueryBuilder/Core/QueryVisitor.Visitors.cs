namespace KISS.QueryBuilder.Core;

/// <summary>
///     The Visitor implements several versions of the same behaviors, tailored for different concrete element classes.
/// </summary>
internal sealed partial class QueryVisitor : IVisitor
{
    /// <inheritdoc />
    public void Visit(SelectComponent element)
    {
        Append(ClauseConstants.Select);

        switch (element.Selector)
        {
            case null:
                Append("*");
                break;

            case MemberExpression memberExpression:
                Translate(memberExpression);
                break;

            case NewExpression newExpression:
                Translate(newExpression);
                break;

            case MemberInitExpression memberInitExpression:
                Translate(memberInitExpression);
                break;

            default:
                throw new NotSupportedException("Expression not supported.");
        }
    }

    /// <inheritdoc />
    public void Visit(SelectDistinctComponent element)
    {
        Append(ClauseConstants.SelectDistinct);

        switch (element.Selector)
        {
            case MemberExpression memberExpression:
                Translate(memberExpression);
                break;

            case NewExpression newExpression:
                Translate(newExpression);
                break;

            default:
                throw new NotSupportedException("Expression not supported.");
        }
    }

    /// <inheritdoc />
    public void Visit(SelectFromComponent element)
    {
        Append(ClauseConstants.From);
        AppendTableAlias(element.Recordset);
    }

    /// <inheritdoc />
    public void Visit(JoinComponent element)
    {
        switch (element.MapSelector)
        {
            case MemberExpression memberExpression:
                {
                    var relationParam = Expression.Parameter(element.Relation, GetTableAlias(element.Relation));
                    var currentProperty = Expression.Property(ReturnParam, memberExpression.Member.Name);

                    var memberType = memberExpression.Type;
                    var isList = memberType != typeof(string)
                                 && memberType.IsGenericType
                                 && memberType.GetGenericTypeDefinition() == typeof(List<>);
                    if (isList)
                    {
                        var listType = typeof(List<>).MakeGenericType(element.Relation);
                        var newList = Expression.IfThen(
                            Expression.Equal(currentProperty, Expression.Constant(null)),
                            Expression.Assign(currentProperty, Expression.New(listType)));

                        // Add relation to mapSelector if it exists
                        var addToList = Expression.Block(
                            newList,
                            Expression.Call(currentProperty, "Add", null, relationParam));

                        var ifExists = Expression.IfThen(
                            Expression.NotEqual(relationParam, Expression.Constant(null)),
                            addToList);

                        BlockMapSequence.Add((relationParam, ifExists));
                    }
                    else
                    {
                        var assignExpression = Expression.Assign(currentProperty, relationParam);
                        BlockMapSequence.Add((relationParam, assignExpression));
                    }

                    break;
                }

            default:
                throw new NotSupportedException("Expression not supported.");
        }

        Append(ClauseConstants.Join);
        AppendTableAlias(element.Relation);
        Append(ClauseConstants.OnSeparator);

        Translate(element.LeftKeySelector);
        Append(BinaryOperandMap[ExpressionType.Equal]);
        Translate(element.RightKeySelector);
    }

    /// <inheritdoc />
    public void Visit(WhereComponent element)
    {
        if (ClauseActions.Contains(ClauseAction.Where))
        {
            Append(ClauseConstants.AndSeparator);
        }
        else
        {
            ClauseActions.Add(ClauseAction.Where);
            Append(ClauseConstants.Where);
        }

        Translate(element.Predicate);
    }

    /// <inheritdoc />
    public void Visit(GroupByComponent element)
    {
        Append(ClauseConstants.GroupBy);
        Translate(element.KeySelector);
    }
}
