namespace KISS.Job.Quartz;

/// <summary>
///     Initializes a new instance of the QuartzOptions by using configuration file settings.
/// </summary>
public abstract class QuartzDefaultOptions
{
    /// <summary>
    ///     This maintains the uniqueness for all schedulers working.
    /// </summary>
    public virtual string SchedulerId { get; set; } = "SYS_PROP";
}
