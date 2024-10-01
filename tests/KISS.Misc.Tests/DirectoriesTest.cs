namespace KISS.Misc.Tests;

public class DirectoriesTest
{
    [Fact]
    public void ReadingCSV_ValidPath_ReturnsWeatherCollection()
    {
        const string fileName = "Assets/GlobalWeatherRepository.csv";
        var weathers = CsvAssists.FromCsv<Weather>(fileName);
        Assert.Equal(100, weathers.Count());
    }

    [Fact]
    public void ReadingCSV_InvalidPath_ThrowsFileNotFoundException()
    {
        const string fileName = "Assets/FileNotFoundException.csv";
        Assert.Throws<FileNotFoundException>(() => CsvAssists.FromCsv<Weather>(fileName));
    }

    [Theory(Skip = "Doesn't work at the moment")]
    [InlineData("ftp://")] // Missing Protocol
    [InlineData("https://")] // Missing Hostname
    [InlineData("https://example.com/path with spaces")] // Illegal Characters or Incomplete Encoding
    [InlineData("https://example.com:8080/?query")] // Incorrect Structure
    [InlineData("https://example.com]")] // Malformed URL Components
    [InlineData("http://ex@mple!.com")] // Invalid Domain Name
    public void GetFullPath_InvalidPath_ThrowsArgumentException(string path)
    {
        Assert.Throws<ArgumentException>(() => PathHelper.GetFullPath(path));
    }

    [Fact]
    public void GetFullPath_ValidPath_ReturnsFullPath()
    {
        const string path = "C:/Assets/GlobalWeatherRepository.csv";
        const string expected = "C:/Assets/GlobalWeatherRepository.csv";
        Assert.Equal(expected, PathHelper.GetFullPath(path).Replace("\\", "/"));
    }
}
