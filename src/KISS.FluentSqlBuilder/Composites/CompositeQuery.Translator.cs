namespace KISS.FluentSqlBuilder.Composites;

/// <summary>
///     A class that defines the fluent SQL builder type.
/// </summary>
/// <typeparam name="TSource">The type representing the database record set.</param>
/// <typeparam name="TReturn">The combined type to return.</param>
public sealed partial class CompositeQuery<TSource, TReturn>
{
    private void SetSelect()
    {
        Append("SELECT");
        AppendLine(true);

        using var selectEnumerator = SelectComponents.GetEnumerator();
        SelectTranslator translator = new(this);

        if (selectEnumerator.MoveNext())
        {
            translator.Translate(selectEnumerator.Current);

            while (selectEnumerator.MoveNext())
            {
                AppendLine(true);
                Append(", ");
                translator.Translate(selectEnumerator.Current);
            }
        }
        else
        {
            if (GroupByComponents.Count != 0)
            {
                using var profilesEnumerator = MapProfiles.Values.AsEnumerable().GetEnumerator();
                if (profilesEnumerator.MoveNext())
                {
                    var (alias, columns) = profilesEnumerator.Current;
                    using var colsEnumerator = columns.AsEnumerable().GetEnumerator();

                    if (colsEnumerator.MoveNext())
                    {
                        Append($"{alias}.{colsEnumerator.Current.Name} AS {alias}_{colsEnumerator.Current.Name}");

                        while (colsEnumerator.MoveNext())
                        {
                            AppendLine(true);
                            Append(", ");
                            Append($"{alias}.{colsEnumerator.Current.Name} AS {alias}_{colsEnumerator.Current.Name}");
                        }
                    }

                    while (profilesEnumerator.MoveNext())
                    {
                        AppendLine(true);
                        Append(", ");

                        var (aliasNext, columnsNext) = profilesEnumerator.Current;
                        using var colsNextEnumerator = columnsNext.AsEnumerable().GetEnumerator();

                        if (colsNextEnumerator.MoveNext())
                        {
                            Append($"{aliasNext}.{colsNextEnumerator.Current.Name} AS {aliasNext}_{colsNextEnumerator.Current.Name}");

                            while (colsNextEnumerator.MoveNext())
                            {
                                AppendLine(true);
                                Append(", ");
                                Append($"{aliasNext}.{colsNextEnumerator.Current.Name} AS {aliasNext}_{colsNextEnumerator.Current.Name}");
                            }
                        }
                    }
                }
            }
            else
            {
                Append("*");
            }
        }

        AppendLine();
    }

    private void SetFrom()
    {
        Append("FROM");
        AppendLine(true);

        using var enumerator = SelectFromComponents.GetEnumerator();
        if (enumerator.MoveNext())
        {
            SelectFromTranslator translator = new(this);
            translator.Translate(enumerator.Current);
        }

        AppendLine();
    }

    private void SetJoin()
    {
        using var enumerator = JoinComponents.GetEnumerator();
        if (enumerator.MoveNext())
        {
            JoinTranslator translator = new(this);

            Append("INNER JOIN");
            Append($" {enumerator.Current.Recordset.Name}s {GetAliasMapping(enumerator.Current.Recordset)} ");
            Append("ON ");
            translator.Translate(enumerator.Current.LeftKeySelector);
            Append(" = ");
            translator.Translate(enumerator.Current.RightKeySelector);

            while (enumerator.MoveNext())
            {
                AppendLine();
                Append("INNER JOIN");
                Append($" {enumerator.Current.Recordset.Name}s {GetAliasMapping(enumerator.Current.Recordset)} ");
                Append("ON ");
                translator.Translate(enumerator.Current.LeftKeySelector);
                Append(" = ");
                translator.Translate(enumerator.Current.RightKeySelector);
            }

            AppendLine();
        }
    }

    private void SetWhere()
    {
        using var enumerator = WhereComponents.GetEnumerator();
        if (enumerator.MoveNext())
        {
            Append("WHERE");
            AppendLine(true);

            WhereTranslator translator = new(this);
            translator.Translate(enumerator.Current);

            while (enumerator.MoveNext())
            {
                AppendLine(true);
                Append("AND");
                translator.Translate(enumerator.Current);
            }

            AppendLine();
        }
    }

    private void SetGroupBy()
    {
        using var selectGroupByEnumerator = GroupByComponents.GetEnumerator();
        using var selectAggregationEnumerator = SelectAggregationComponents.GetEnumerator();
        if (selectGroupByEnumerator.MoveNext())
        {
            SqlBuilder.Insert(0, "WITH CommonTableExpression AS (");
            Append(")");
            AppendLine();

            GroupByTranslator translator = new(this);
            translator.Translate(selectGroupByEnumerator.Current);

            while (selectGroupByEnumerator.MoveNext())
            {
                translator.Translate(selectGroupByEnumerator.Current);
            }

            var gBy = string.Join(", ", GroupKeys.Select(k => k.GroupKey));
            var pBy = string.Join(", ", GroupKeys.Select(k => $"GP.{k.GroupKey}"));
            var onKeys = string.Join(" AND ", GroupKeys.Select(k => $"CTE.{k.GroupKey} = GP.{k.GroupKey}"));
            var selectAggregation = string.Join(", ", SelectAggregationComponents.Select(a => $"GP.{a.Alias}"));
            var aggregation = string.Join(", ", SelectAggregationComponents.Select((a, i) => $"{a.AggregationType}({{{i}}}) AS {a.Alias}"));

            if (selectAggregationEnumerator.MoveNext())
            {
                selectAggregation += ",";
                GroupKeys.Clear();
                translator.Translate(selectAggregationEnumerator.Current.Expr);

                while (selectAggregationEnumerator.MoveNext())
                {
                    translator.Translate(selectAggregationEnumerator.Current.Expr);
                }

                aggregation = $", {string.Format(aggregation, [.. GroupKeys.Select(k => k.GroupKey)])}";
            }

            Append($@"
                SELECT 
                    ROW_NUMBER() OVER (PARTITION BY {pBy} ORDER BY {pBy} DESC) AS RowNum,
                    {selectAggregation}
                    CTE.*
                FROM CommonTableExpression CTE
                JOIN (
                    SELECT 
                        {gBy}
                        {aggregation}
                    FROM CommonTableExpression
                    GROUP BY {gBy}
                ) GP
                ON {onKeys};
            ");

            AppendLine();
        }
    }

    // private void SetHaving()
    // {
    //     using var enumerator = HavingComponents.GetEnumerator();
    //     if (enumerator.MoveNext())
    //     {
    //         Append("HAVING");
    //         AppendLine(true);
    //         HavingTranslator translator = new(this);
    //         translator.Translate(enumerator.Current);
    //         while (enumerator.MoveNext())
    //         {
    //             AppendLine(true);
    //             Append("AND");
    //             translator.Translate(enumerator.Current);
    //         }
    //         AppendLine();
    //     }
    // }

    private void SetOrderBy()
    {
        using var enumerator = OrderByComponents.GetEnumerator();
        if (enumerator.MoveNext())
        {
            Append("ORDER BY");
            AppendLine(true);

            OrderByTranslator translator = new(this);
            translator.Translate(enumerator.Current);

            AppendLine();
        }
    }

    private void SetLimit()
    {
        using var enumerator = LimitComponents.GetEnumerator();
        if (enumerator.MoveNext())
        {
            Append("LIMIT");
            AppendLine(true);

            LimitTranslator translator = new(this);
            translator.Translate(enumerator.Current);

            AppendLine();
        }
    }

    private void SetOffset()
    {
        using var enumerator = OffsetComponents.GetEnumerator();
        if (enumerator.MoveNext())
        {
            Append("OFFSET");
            AppendLine(true);

            OffsetTranslator translator = new(this);
            translator.Translate(enumerator.Current);

            AppendLine();
        }
    }
}
