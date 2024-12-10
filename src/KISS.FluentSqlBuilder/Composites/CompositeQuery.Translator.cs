namespace KISS.FluentSqlBuilder.Composites;

/// <summary>
///     A class that defines the fluent SQL builder type.
/// </summary>
public sealed partial class CompositeQuery
{
    private void SetSelect()
    {
        Append("SELECT");
        AppendLine(true);

        using var enumerator = SelectComponents.GetEnumerator();
        if (enumerator.MoveNext())
        {
            SelectTranslator translator = new(this);
            translator.Translate(enumerator.Current);

            while (enumerator.MoveNext())
            {
                Append(", ");
                translator.Translate(enumerator.Current);
            }
        }
        else
        {
            Append("Extend0.*");
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
        using var enumerator = GroupByComponents.GetEnumerator();
        if (enumerator.MoveNext())
        {
            Append("GROUP BY");
            AppendLine(true);

            GroupByTranslator translator = new(this);
            translator.Translate(enumerator.Current);

            AppendLine();
        }
    }

    private void SetHaving()
    {
        using var enumerator = HavingComponents.GetEnumerator();
        if (enumerator.MoveNext())
        {
            Append("HAVING");
            AppendLine(true);

            HavingTranslator translator = new(this);
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
