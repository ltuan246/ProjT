namespace KISS.GuardClauses;

public static class Ensure
{
    public static bool IsNull<T>([NotNullWhen(false)] T? value)
        where T : struct
        => value.HasValue is false;

    public static bool IsNull<T>([NotNullWhen(false)] T? value)
        => value is null;

    public static bool IsNullOrEmpty([NotNullWhen(false)] string? value)
        => string.IsNullOrEmpty(value);

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] string? value)
        => string.IsNullOrWhiteSpace(value);

    public static bool IsNullOrEmpty([NotNullWhen(false)] Guid? value)
        => value is null || IsEqualTo(value, Guid.Empty);

    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] IEnumerable<T>? values)
        => values switch
        {
            null => true,
            Array and { Length: 0 } => true,
            ICollection<T> and { Count: 0 } => true,
            _ => values.TryGetNonEnumeratedCount(out int count) && count == 0
        };

    public static bool IsNotNull<T>([NotNullWhen(true)] T? value)
        => value is not null;

    public static bool IsNotNullOrEmpty([NotNullWhen(true)] string? value)
        => !string.IsNullOrEmpty(value);

    public static bool IsNotNullOrEmpty([NotNullWhen(true)] Guid? value)
        => value.HasValue && !IsEqualTo(value, Guid.Empty);

    public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] string? value)
        => !string.IsNullOrWhiteSpace(value);

    public static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] IEnumerable<T?>? values)
        => !IsNullOrEmpty(values);

    public static bool IsNotNullOrEmptyAndDoesNotContainAnyNulls<T>([NotNullWhen(true)] IEnumerable<T?>? values)
        => values switch
        {
            null => false,
            Array and { Length: 0 } => false,
            ICollection<T> and { Count: 0 } => false,
            _ => values.TryGetNonEnumeratedCount(out int count)
                 && count > 0
                 && values.All(val => val is not null)
        };

    public static bool IsEqualTo<T>(T value, T comparand)
        => value?.Equals(comparand) is true;

    public static bool IsBetween<T>(T value, T min, T max)
        where T : IComparable<T>
        => value.CompareTo(min) > 0 && value.CompareTo(max) < 0;

    public static bool IsGreaterThan<T>(T value, T comparand)
        where T : IComparable<T>
        => value.CompareTo(comparand) > 0;

    public static bool IsGreaterThanOrEqualTo<T>(T value, T comparand)
        where T : IComparable<T>
        => value.CompareTo(comparand) >= 0;

    public static bool IsLessThan<T>(T value, T comparand)
        where T : IComparable<T>
        => value.CompareTo(comparand) < 0;

    public static bool IsLessThanOrEqualTo<T>(T value, T comparand)
        where T : IComparable<T>
        => value.CompareTo(comparand) <= 0;

    public static bool IsDefined<T>(int value)
        where T : Enum
        => Enum.IsDefined(typeof(T), value);

    public static bool IsContainsKey<TKey, TValue>(IDictionary<TKey, TValue> dic, TKey key)
        => dic?.ContainsKey(key) == true;
}