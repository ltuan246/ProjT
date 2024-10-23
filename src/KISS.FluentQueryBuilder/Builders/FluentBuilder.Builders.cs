namespace KISS.FluentQueryBuilder.Builders;

/// <summary>
///     Declares specifies methods for creating the different parts of the <see cref="FluentBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public sealed partial class FluentBuilder<TEntity>
{
    private StringBuilder SqlBuilder { get; } = new();

    private SqlFormatter Formatter { get; } = new();

    private Stack<Expression> ExpressionStack { get; } = new();

    private Dictionary<Expression, bool> Evaluable { get; } = [];

    /// <summary>
    ///     The SQL to execute for the query.
    /// </summary>
    private string Sql
        => SqlBuilder.ToString();

    /// <summary>
    ///     The parameters to pass, if any.
    /// </summary>
    private DynamicParameters Parameters
        => Formatter.Parameters;

    /// <summary>
    ///     The connection to a database.
    /// </summary>
    public required DbConnection Connection { get; init; }

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    private bool HasOpenParentheses { get; set; }

    private void OpenParentheses()
    {
        HasOpenParentheses = true;
        SqlBuilder.Append(BuilderConstants.OpenParentheses);
    }

    private void CloseParentheses()
    {
        if (!HasOpenParentheses)
        {
            return;
        }

        HasOpenParentheses = false;
        SqlBuilder.Append(BuilderConstants.CloseParentheses);
    }

    private void AddCommaSeparated()
        => SqlBuilder.Append(BuilderConstants.Comma);

    private (bool Evaluated, FormattableString Value) GetValue(Expression node)
    {
        if (!Evaluable.TryGetValue(node, out var canEvaluate))
        {
            Visit(node);
            Evaluable.TryGetValue(node, out canEvaluate);
        }

        (var evaluated, FormattableString value) = (canEvaluate, $"");
        if (canEvaluate)
        {
            var lambdaExpression = Expression.Lambda(node);
            value = $"{lambdaExpression.Compile().DynamicInvoke()}";
        }

        return (evaluated, value);
    }

    /// <summary>
    ///     Implements IFormatProvider and ICustomFormatter, which returns string information for supplied objects based on
    ///     custom criteria.
    /// </summary>
    private sealed class SqlFormatter : IFormatProvider, ICustomFormatter
    {
        private const string DefaultDatabaseParameterNameTemplate = "p";
        private const string DefaultDatabaseParameterPrefix = "@";

        /// <summary>
        ///     A dynamic object that can be passed to the Query method instead of normal parameters.
        /// </summary>
        public DynamicParameters Parameters { get; } = new();

        private int ParamCount { get; set; }

        /// <inheritdoc />
        public string Format(string? format, object? arg, IFormatProvider? formatProvider)
            => AddValueToParameters(arg);

        /// <inheritdoc />
        public object GetFormat(Type? formatType) => this;

        private string GetNextParameterName()
            => $"{DefaultDatabaseParameterNameTemplate}{ParamCount++}";

        private string AppendParameterPrefix(string parameterName)
            => $"{DefaultDatabaseParameterPrefix}{parameterName}";

        private string AddValueToParameters<T>(T value)
        {
            var parameterName = GetNextParameterName();
            Parameters.Add(parameterName, value, direction: ParameterDirection.Input);
            return AppendParameterPrefix(parameterName);
        }
    }
}
