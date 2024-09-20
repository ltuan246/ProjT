namespace KISS.Misc.Options;

/// <summary>
///     Implement dynamic dependencies and options automatically based on attributes.
/// </summary>
/// <param name="sectionName">The Section name.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class OptionsAttribute(string sectionName) : Attribute
{
    /// <summary>
    ///     The Section name.
    /// </summary>
    public string SectionName { get; } = sectionName;
}
