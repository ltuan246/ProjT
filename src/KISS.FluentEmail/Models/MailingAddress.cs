namespace KISS.FluentEmail.Models;

/// <summary>
///     Create a new <see cref="MailingAddress" />.
/// </summary>
/// <param name="MailAddress">An email address.</param>
/// <param name="DisplayName">The display name associated with address.</param>
public record MailingAddress(string MailAddress, string? DisplayName)
{
    /// <summary>
    ///     Return the address and display name.
    /// </summary>
    /// <param name="address">An email address.</param>
    /// <param name="displayName">The display name associated with address.</param>
    public void Deconstruct(out string address, out string? displayName)
    {
        address = MailAddress;
        displayName = DisplayName;
    }
}
