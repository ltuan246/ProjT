namespace KISS.Misc.GuardClauses;

public static partial class GuardClauseExtensions
{
    public static T Null<T>(this IGuardClause _,
        [NotNull] T? input,
        string? parameterName = null,
        string? message = null)
        => Ensure.IsNull(input) switch
        {
            true => throw new ArgumentNullException(parameterName, message),
            false => input
        };

    public static T Null<T>(this IGuardClause _,
        [NotNull] T? input,
        string? parameterName = null,
        string? message = null) where T : struct
        => Ensure.IsNull(input) switch
        {
            true => throw new ArgumentNullException(parameterName, message),
            false => input.Value
        };

    public static string NullOrEmpty(this IGuardClause _,
        [NotNull] string input,
        string? parameterName = null,
        string? message = null)
        => Ensure.IsNullOrEmpty(input) switch
        {
            true => throw new ArgumentNullException(parameterName, message),
            false => input
        };

    public static string NullOrEmptyOrWhiteSpace(this IGuardClause _,
        [NotNull] string input,
        string? parameterName = null,
        string? message = null)
        => (Ensure.IsNullOrEmpty(input) || Ensure.IsNullOrWhiteSpace(input)) switch
        {
            true => throw new ArgumentNullException(parameterName, message),
            false => input
        };

    public static Guid NullOrEmpty(this IGuardClause _,
        [NotNull] Guid? input,
        string? parameterName = null,
        string? message = null)
        => Ensure.IsNullOrEmpty(input) switch
        {
            true => throw new ArgumentNullException(parameterName, message),
            false => input.Value
        };

    public static IEnumerable<T> NullOrEmpty<T>(this IGuardClause _,
        [NotNull] IEnumerable<T>? input,
        string? parameterName = null,
        string? message = null)
        => Ensure.IsNullOrEmpty(input) switch
        {
            true => throw new ArgumentNullException(parameterName, message),
            false => input
        };
}