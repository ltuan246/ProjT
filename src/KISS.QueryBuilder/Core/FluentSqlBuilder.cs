namespace KISS.QueryBuilder.Core;

/// <summary>
///     A class that defines the fluent SQL builder type. The core <see cref="FluentSqlBuilder" /> partial class.
/// </summary>
internal sealed partial class FluentSqlBuilder
{
    private StringBuilder SqlBuilder { get; } = new();
}
