namespace KISS.FluentSqlBuilder.Utils;

/// <summary>
/// TypeUtils.
/// </summary>
public sealed record TypeUtils
{
    /// <summary>
    /// IteratorType.
    /// </summary>
    private static Type IteratorType { get; } = typeof(IEnumerator);

    // private static Type IteratorType { get; } = typeof(IDisposable);
    //
    // private static Type IteratorType { get; } = typeof(IEnumerable<IDictionary<string, object>>);

    /// <summary>
    /// IterMoveNextMethod.
    /// </summary>
    public static MethodInfo IterMoveNextMethod { get; } = IteratorType.GetMethod("MoveNext", Type.EmptyTypes)!;
}
