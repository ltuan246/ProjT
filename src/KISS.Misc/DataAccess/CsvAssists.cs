namespace KISS.Misc.DataAccess;

/// <summary>
///     The common helper containing frequently reused functions.
/// </summary>
public static class CsvAssists
{
    /// <summary>
    ///     Reading a CSV File into a List.
    /// </summary>
    /// <param name="path">The fully qualified location of path.</param>
    /// <param name="config">The configuration.</param>
    /// <typeparam name="TEntity">The type of the record.</typeparam>
    /// <returns>Data From a CSV File.</returns>
    /// <exception cref="FileNotFoundException">The exception that is throw if invalid path.</exception>
    public static IEnumerable<TEntity> FromCsv<TEntity>(string path, CsvConfiguration? config = null)
    {
        var filePath = PathHelper.GetFullPath(path);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Invalid path: {filePath}");
        }

        config ??= new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using StreamReader streamReader = new(filePath);
        using CsvReader csvReader = new(streamReader, config);

        return [.. csvReader.GetRecords<TEntity>()];
    }
}
