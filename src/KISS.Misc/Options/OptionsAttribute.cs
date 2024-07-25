namespace KISS.Misc.Options;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class OptionsAttribute(string sectionName) : Attribute
{
    public string SectionName { get; } = sectionName;
}
