namespace KISS.FluentEmail.Models;

/// <summary>
///     An attached file to the message.
/// </summary>
/// <param name="filename">Specific name of this attachment.</param>
/// <param name="data">Content stream of this attachment.</param>
/// <param name="contentType">Content type of this attachment.</param>
public class FileAttachment(string filename, Stream data, string contentType)
{
    /// <summary>
    ///     Specific name of this attachment.
    /// </summary>
    public string Filename { get; } = filename;

    /// <summary>
    ///     Content stream of this attachment.
    /// </summary>
    public Stream Data { get; } = data;

    /// <summary>
    ///     Content type of this attachment.
    /// </summary>
    public string ContentType { get; } = contentType;

    /// <summary>
    ///     Return the specific name, content, and content type of this attachment.
    /// </summary>
    /// <param name="filename">Specific name of this attachment.</param>
    /// <param name="data">Content stream of this attachment.</param>
    /// <param name="contentType">Content type of this attachment.</param>
    public void Deconstruct(out string filename, out Stream data, out string contentType)
    {
        filename = Filename;
        data = Data;
        contentType = ContentType;
    }
}
