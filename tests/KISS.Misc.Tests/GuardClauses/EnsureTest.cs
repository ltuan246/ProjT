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
}