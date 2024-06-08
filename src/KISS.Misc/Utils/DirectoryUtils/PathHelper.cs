namespace KISS.Misc.Utils.DirectoryUtils;

public sealed record PathHelper
{
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