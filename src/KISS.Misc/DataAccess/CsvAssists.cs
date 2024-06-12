namespace KISS.Misc.DataAccess;

public sealed record CsvAssists
{
    public static IEnumerable<TEntity> FromCsv<TEntity>(string path, CsvConfiguration? config = null)
    {
        string filePath = PathHelper.GetFullPath(path);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Invalid path: {filePath}");
        }

        config ??= new(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using StreamReader streamReader = new(filePath);
        using CsvReader csvReader = new(streamReader, config);

        return csvReader.GetRecords<TEntity>().ToList();
    }
}