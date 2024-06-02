namespace KISS.Misc.Tests.GuardClauses;

public class EnsureTest
{
    [Fact]
    public void ReturnsExpectedValueWhenGivenNullValue()
    {
        string? nullString = null;
        Assert.True(Ensure.IsNull(nullString));
        Assert.True(Ensure.IsNullOrEmpty(nullString));
        Assert.True(Ensure.IsNullOrEmpty(""));
        Assert.True(Ensure.IsNullOrWhiteSpace(nullString));
        Assert.True(Ensure.IsNullOrWhiteSpace(""));
        Assert.True(Ensure.IsNullOrWhiteSpace("  "));

        Guid? nullGuid = null;
        Assert.True(Ensure.IsNull(nullGuid));
        Assert.True(Ensure.IsNullOrEmpty(nullGuid));
        Assert.True(Ensure.IsNullOrEmpty(Guid.Empty));

        object[]? nullEnumerable = null;
        Assert.True(Ensure.IsNull(nullEnumerable));
        Assert.True(Ensure.IsNullOrEmpty(nullEnumerable));
        Assert.True(Ensure.IsNullOrEmpty(Array.Empty<object>()));
        Assert.True(Ensure.IsNullOrEmpty(Enumerable.Empty<object>()));
        Assert.True(Ensure.IsNullOrEmpty(new List<object?>()));

        Assert.False(Ensure.IsNotNullOrEmptyAndDoesNotContainAnyNulls(nullEnumerable));
        Assert.False(Ensure.IsNotNullOrEmptyAndDoesNotContainAnyNulls(Array.Empty<object>()));
        Assert.False(Ensure.IsNotNullOrEmptyAndDoesNotContainAnyNulls(Enumerable.Empty<object>()));
        Assert.False(Ensure.IsNotNullOrEmptyAndDoesNotContainAnyNulls(new List<object?> { null }));
    }

    [Fact]
    public void ReturnsExpectedValueWhenGivenNonNullValue()
    {
        Assert.True(Ensure.IsNotNull(""));
        Assert.True(Ensure.IsNotNullOrEmpty(" "));
        Assert.True(Ensure.IsNotNullOrWhiteSpace("a"));

        Assert.True(Ensure.IsNotNull(Guid.NewGuid()));
        Assert.True(Ensure.IsNotNullOrEmpty(Guid.NewGuid()));

        string[] nonNullEnumerable = ["a"];
        Assert.True(Ensure.IsNotNull(nonNullEnumerable));
        Assert.True(Ensure.IsNotNullOrEmpty(nonNullEnumerable));

        Assert.True(Ensure.IsNotNullOrEmptyAndDoesNotContainAnyNulls(nonNullEnumerable));
        Assert.True(Ensure.IsNotNullOrEmptyAndDoesNotContainAnyNulls(new List<object?> { "a" }));
    }

    [Theory]
    [InlineData(1, 10)]
    public void ReturnsComparingExpectedValue(int lesserValue, int greaterValue)
    {
        Assert.True(Ensure.IsGreaterThan(greaterValue, lesserValue));
        Assert.True(Ensure.IsGreaterThanOrEqualTo(greaterValue, lesserValue));
        Assert.True(Ensure.IsLessThan(lesserValue, greaterValue));
        Assert.True(Ensure.IsLessThanOrEqualTo(lesserValue, greaterValue));
        Assert.True(Ensure.IsBetween(Enumerable.Range(lesserValue, greaterValue).Average(), lesserValue, greaterValue));
    }

    [Theory]
    [InlineData(10, 10)]
    public void ReturnsExpectedValueWhenGivenEqualValue(int actual, int expected)
    {
        Assert.True(Ensure.IsEqualTo(actual, expected));
        Assert.False(Ensure.IsGreaterThan(actual, expected));
        Assert.True(Ensure.IsGreaterThanOrEqualTo(actual, expected));
        Assert.False(Ensure.IsLessThan(actual, expected));
        Assert.True(Ensure.IsLessThanOrEqualTo(actual, expected));
    }

    [Theory]
    [InlineData(1, 10)]
    public void ReturnsExpectedValueWhenGivenNotEqualValue(int actual, int expected)
    {
        Assert.False(Ensure.IsEqualTo(actual, expected));
    }

    [Fact]
    public void ReturnsExpectedValueWhenGivenDefinedValueOrElse()
    {
        Assert.True(Ensure.IsDefined<ServiceLifetime>(0));
        Assert.False(Ensure.IsDefined<ServiceLifetime>(99));

        Dictionary<int, int> dic = new()
        {
            [0] = 0,
            [1] = 1,
            [2] = 2
        };

        Assert.True(Ensure.IsContainsKey(dic, 0));
        Assert.False(Ensure.IsContainsKey(dic, 99));
    }
}