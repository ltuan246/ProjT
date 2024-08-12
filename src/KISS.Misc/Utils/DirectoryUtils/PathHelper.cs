namespace KISS.Misc.Utils.DirectoryUtils;

/// <summary>
/// The common helper containing frequently reused functions.
/// </summary>
public static class PathHelper
{
    /// <summary>
    /// Returns the absolute path for the specified path string.
    /// </summary>
    /// <param name="path">The file or directory for which to obtain absolute path information.</param>
    /// <returns>The fully qualified location of path.</returns>
    /// <exception cref="ArgumentException">The exception that is throw if invalid path.</exception>
    public static string GetFullPath(string path)
    {
        Guard.Against.NullOrEmptyOrWhiteSpace(path);

        path = path.Replace('\\', Path.DirectorySeparatorChar);
        path = path.Replace('/', Path.DirectorySeparatorChar);

        Guard.Against.NullOrEmptyOrWhiteSpace(path);

        if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out Uri? pathUri))
        {
            throw new ArgumentException($"Invalid path: {path}");
        }

        if (pathUri.IsAbsoluteUri)
        {
            return path;
        }

        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

        return path;
    }
}
