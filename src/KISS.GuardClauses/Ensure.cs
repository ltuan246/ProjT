namespace KISS.GuardClauses;

/// <summary>
/// A collection of common guard clauses, implemented as extensions.
/// </summary>
public static class Ensure
{
    /// <summary>
    /// The method used to check whether the value is null.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if the current object has no value; false if the current object has a value.</returns>
    public static bool IsNull<T>([NotNullWhen(false)] T? value)
        where T : struct
        => value.HasValue is false;

    /// <summary>
    /// The method used to check whether the value is null.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if the current object has no value; false if the current object has a value.</returns>
    public static bool IsNull<T>([NotNullWhen(false)] T? value)
        => value is null;

    /// <summary>
    /// The method used to check whether the value is null or empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>True if the current object has no value; false if the current object has a value.</returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] string? value)
        => string.IsNullOrEmpty(value);

    /// <summary>
    /// The method used to check whether the value is null or WhiteSpace.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>True if the current object has no value; false if the current object has a value.</returns>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] string? value)
        => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// The method used to check whether the value is null or empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>True if the current object has no value; false if the current object has a value.</returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] Guid? value)
        => value is null || IsEqualTo(value, Guid.Empty);

    /// <summary>
    /// The method used to check whether the value is null or empty.
    /// </summary>
    /// <param name="values">The value.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if the current object has no value; false if the current object has a value.</returns>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] IEnumerable<T>? values)
        => values switch
        {
            null => true,
            Array and { Length: 0 } => true,
            ICollection<T> and { Count: 0 } => true,
            _ => values.TryGetNonEnumeratedCount(out int count) && count == 0
        };

    /// <summary>
    /// The method used to check whether the value is not null.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if the current object has a value; false if the current object has no value.</returns>
    public static bool IsNotNull<T>([NotNullWhen(true)] T? value)
        => value is not null;

    /// <summary>
    /// The method used to check whether the value is not null or empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>True if the current object has a value; false if the current object has no value.</returns>
    public static bool IsNotNullOrEmpty([NotNullWhen(true)] string? value)
        => !string.IsNullOrEmpty(value);

    /// <summary>
    /// The method used to check whether the value is not null or empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>True if the current object has a value; false if the current object has no value.</returns>
    public static bool IsNotNullOrEmpty([NotNullWhen(true)] Guid? value)
        => value.HasValue && !IsEqualTo(value, Guid.Empty);

    /// <summary>
    /// The method used to check whether the value is not null or WhiteSpace.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>True if the current object has a value; false if the current object has no value.</returns>
    public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] string? value)
        => !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// The method used to check whether the value is not null or empty.
    /// </summary>
    /// <param name="values">The value.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if the current object has a value; false if the current object has no value.</returns>
    public static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] IEnumerable<T?>? values)
        => !IsNullOrEmpty(values);

    /// <summary>
    /// The method used to check whether the value is not null.
    /// </summary>
    /// <param name="values">The value.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if the current object has a value; false if the current object has no value.</returns>
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

    /// <summary>
    /// The method used to check whether the value is equal to the comparand.
    /// </summary>
    /// <param name="value">The current object.</param>
    /// <param name="comparand">The object to compare with the current object.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if the current object is equal to the specified object; otherwise, false.</returns>
    public static bool IsEqualTo<T>(T value, T comparand)
        => value?.Equals(comparand) is true;

    /// <summary>
    /// The method used to check whether the value is between the comparand.
    /// </summary>
    /// <param name="value">The current object.</param>
    /// <param name="min">The begin.</param>
    /// <param name="max">The end.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if the current object is between to the comparand; otherwise, false.</returns>
    public static bool IsBetween<T>(T value, T min, T max)
        where T : IComparable<T>
        => value.CompareTo(min) > 0 && value.CompareTo(max) < 0;

    /// <summary>
    /// The method used to check whether the value is greater than the comparand.
    /// </summary>
    /// <param name="value">The current object.</param>
    /// <param name="comparand">The object to compare with the current object.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if the current object is greater than the specified object; otherwise, false.</returns>
    public static bool IsGreaterThan<T>(T value, T comparand)
        where T : IComparable<T>
        => value.CompareTo(comparand) > 0;

    /// <summary>
    /// The method used to check whether the value is greater than or equal to the comparand.
    /// </summary>
    /// <param name="value">The current object.</param>
    /// <param name="comparand">The object to compare with the current object.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if the current object is greater than or equal to the specified object; otherwise, false.</returns>
    public static bool IsGreaterThanOrEqualTo<T>(T value, T comparand)
        where T : IComparable<T>
        => value.CompareTo(comparand) >= 0;

    /// <summary>
    /// The method used to check whether the value is less than the comparand.
    /// </summary>
    /// <param name="value">The current object.</param>
    /// <param name="comparand">The object to compare with the current object.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if the current object is less than the specified object; otherwise, false.</returns>
    public static bool IsLessThan<T>(T value, T comparand)
        where T : IComparable<T>
        => value.CompareTo(comparand) < 0;

    /// <summary>
    /// The method used to check whether the value is less than or equal to the comparand.
    /// </summary>
    /// <param name="value">The current object.</param>
    /// <param name="comparand">The object to compare with the current object.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if the current object is less than or equal to the specified object; otherwise, false.</returns>
    public static bool IsLessThanOrEqualTo<T>(T value, T comparand)
        where T : IComparable<T>
        => value.CompareTo(comparand) <= 0;

    /// <summary>
    /// The method used to check whether a given integral value, or its name as a string, exists in a specified enumeration.
    /// </summary>
    /// <param name="value">The current object.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns>True if a constant in enumType has a value equal to value; otherwise, false.</returns>
    public static bool IsDefined<T>(int value)
        where T : Enum
        => Enum.IsDefined(typeof(T), value);

    /// <summary>
    /// The method used to check whether the Dictionary contains an element with the specified key.
    /// </summary>
    /// <param name="dic">The current Dictionary.</param>
    /// <param name="key">The key to locate in the Dictionary.</param>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>True if contains an element with the specified key; otherwise, false.</returns>
    public static bool IsContainsKey<TKey, TValue>(IDictionary<TKey, TValue> dic, TKey key)
        => dic?.ContainsKey(key) == true;
}
