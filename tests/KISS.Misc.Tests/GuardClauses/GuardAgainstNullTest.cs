namespace KISS.Misc.Tests.GuardClauses;

public class GuardAgainstNullTest
{
    [Fact]
    public void DoesNothingGivenNonNullValue()
    {
        Guard.Against.Null("", "string");
        Guard.Against.Null(1, "int");
        Guard.Against.Null(Guid.Empty, "guid");
        Guard.Against.Null(DateTime.Now, "datetime");
        Guard.Against.Null(new object(), "object");
    }

    [Fact]
    public void ThrowsGivenNullValue()
    {
        object obj = null!;
        Assert.Throws<ArgumentNullException>(() => Guard.Against.Null(obj));

        Guid? nullGuid = null!;
        Assert.Throws<ArgumentNullException>(() => Guard.Against.Null(nullGuid));
    }

    [Fact]
    public void ReturnsExpectedValueWhenGivenNonNullValue()
    {
        Assert.Equal("", Guard.Against.Null("", "string"));
        Assert.Equal(1, Guard.Against.Null(1, "int"));

        Guid? guid = Guid.Empty;
        Assert.Equal(guid, Guard.Against.Null(guid, "guid"));

        DateTime now = DateTime.Now;
        Assert.Equal(now, Guard.Against.Null(now, "datetime"));

        object obj = new();
        Assert.Equal(obj, Guard.Against.Null(obj, "object"));
    }

    [Theory]
    [InlineData(null, "Exception of type 'System.ArgumentNullException' was thrown. (Parameter 'parameterName')")]
    [InlineData("Please provide correct value", "Please provide correct value (Parameter 'parameterName')")]
    public void ErrorMessageMatchesExpected(string? customMessage, string? expectedMessage)
    {
        string? nullString = null;
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() => Guard.Against.Null(nullString, "parameterName", customMessage));
        Assert.NotNull(exception);
        Assert.NotNull(exception.Message);
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "Please provide correct value")]
    [InlineData("SomeParameter", null)]
    [InlineData("SomeOtherParameter", "Value must be correct")]
    public void ExceptionParamNameMatchesExpected(string? expectedParamName, string? customMessage)
    {
        string? nullString = null;
        var exception =
            Assert.Throws<ArgumentNullException>(() =>
                Guard.Against.Null(nullString, expectedParamName, customMessage));
        Assert.NotNull(exception);
        Assert.Equal(expectedParamName, exception.ParamName);
    }

    [Fact]
    public void DoesNothingGivenNonEmptyValue()
    {
        Guard.Against.NullOrEmpty("a", "string");
        Guard.Against.NullOrEmpty("1", "aNumericString");
        Guard.Against.NullOrEmpty(Guid.NewGuid(), "guid");
        Guard.Against.NullOrEmpty(["foo", "bar"], "stringArray");
        Guard.Against.NullOrEmpty([1, 2], "intArray");
        Guard.Against.NullOrEmptyOrWhiteSpace("a", "string");
    }

    [Fact]
    public void ThrowsGivenEmptyValue()
    {
        Assert.Throws<ArgumentNullException>(() => Guard.Against.NullOrEmpty("", "emptyString"));
        Assert.Throws<ArgumentNullException>(() => Guard.Against.NullOrEmpty(Guid.Empty, "emptyGuid"));
        Assert.Throws<ArgumentNullException>(() =>
            Guard.Against.NullOrEmpty(Enumerable.Empty<string>(), "emptyStringEnumerable"));
    }

    [Fact]
    public void ReturnsExpectedValueWhenGivenValidValue()
    {
        Assert.Equal("a", Guard.Against.NullOrEmpty("a", "string"));
        Assert.Equal("1", Guard.Against.NullOrEmpty("1", "aNumericString"));

        string[] collection1 = ["foo", "bar"];
        Assert.Equal(collection1, Guard.Against.NullOrEmpty(collection1, "stringArray"));

        int[] collection2 = [1, 2];
        Assert.Equal(collection2, Guard.Against.NullOrEmpty(collection2, "intArray"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void ThrowsGivenWhiteSpaceString(string? whiteSpaceString)
    {
        Assert.Throws<ArgumentNullException>(() =>
            Guard.Against.NullOrEmptyOrWhiteSpace(whiteSpaceString));
    }
}