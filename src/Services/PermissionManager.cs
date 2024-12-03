namespace Tavenem.Wiki.Services;

/// <summary>
/// A default implementation of <see cref="IPermissionManager"/> which always returns <see langword="null"/>.
/// </summary>
public class PermissionManager : IPermissionManager
{
    /// <summary>
    /// When a user attempts to interact with an article in a domain (including viewing, creating,
    /// editing, or deleting items), this function is invoked to determine the permissions the user
    /// has for that domain.
    /// </summary>
    /// <param name="userId">The ID of a user.</param>
    /// <param name="domain">The domain.</param>
    /// <returns>
    /// The permissions the user has for the given <paramref name="domain"/>; or <see
    /// langword="null"/> to indicate that permission determination should defer to other sources
    /// (i.e. that this method gives no information for the given user).
    /// </returns>
    /// <remarks>
    /// <para>
    /// The <see cref="WikiUser.AllowedViewDomains"/> property for the given user will also be
    /// checked, and will provide <see cref="WikiPermission.Read"/> permission, if a matching domain
    /// name is found.
    /// </para>
    /// <para>
    /// The user's effective permission is determined by the combination of this function, <see
    /// cref="WikiUser.AllowedViewDomains"/>, and <see cref="WikiGroup.AllowedViewDomains"/>, as
    /// well as any access controls on the specific article, which override the general permissions
    /// for the domain, if present.
    /// </para>
    /// <para>
    /// Note that the default when no permission is specified is to be denied access (unlike the
    /// default for non-domain articles, which is to grant full access even to anonymous users).
    /// </para>
    /// <para>
    /// Also see <seealso cref="WikiOptions.UserDomains"/>.
    /// </para>
    /// </remarks>
    public ValueTask<WikiPermission?> GetDomainPermissionAsync(string userId, string domain)
        => ValueTask.FromResult<WikiPermission?>(null);
}
