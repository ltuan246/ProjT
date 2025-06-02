namespace KISS.FluentSqlBuilder.Core;

/// <summary>
///     Specifies the exact table name in the database for a class when used with FluentSqlBuilder.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SqlTableAttribute(string name) : Attribute
{
    /// <summary>
    ///     The name of the table in the database.
    /// </summary>
    public string Name { get; } = name;
}

/// <summary>
///     Specifies the exact table name in the database for a class when used with FluentSqlBuilder.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class KeyBuilderAttribute : Attribute;
