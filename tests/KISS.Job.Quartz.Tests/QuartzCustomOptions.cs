namespace KISS.Job.Quartz.Tests;

/// <summary>
///     Initializes a new instance of the QuartzOptions by using configuration file settings.
/// </summary>
public class QuartzCustomOptions : QuartzDefaultOptions
{
    /// <summary>
    ///     This maintains the uniqueness for all schedulers working.
    /// </summary>
    public override string SchedulerId { get; set; } = "AUTO";
}
