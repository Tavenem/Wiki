using System.Security.Claims;

namespace Tavenem.Wiki;

/// <summary>
/// A user manager interface for <see cref="IWikiUser"/>s.
/// </summary>
public interface IWikiUserManager
{
    /// <summary>
    /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">The user ID to search for.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing the user
    /// matching the specified <paramref name="userId"/> if it exists.
    /// </returns>
    ValueTask<IWikiUser?> FindByIdAsync(string? userId);

    /// <summary>
    /// <para>
    /// Finds and returns a user, if any, who has the specified user name.
    /// </para>
    /// <para>
    /// Returns <see langword="null"/> if there is more than one user with the specified name.
    /// In other words, this only returns a result if the given name has a unique match.
    /// </para>
    /// </summary>
    /// <param name="userName">The user name to search for.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing the user
    /// matching the specified <paramref name="userName"/> if it exists.
    /// </returns>
    ValueTask<IWikiUser?> FindByNameAsync(string? userName);

    /// <summary>
    /// Returns the user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType
    /// claim in the <paramref name="principal"/> or <see langword="null"/>.
    /// </summary>
    /// <param name="principal">The principal which contains the user id claim.</param>
    /// <returns>
    /// The user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType claim in
    /// the <paramref name="principal"/> or <see langword="null"/>
    /// </returns>
    ValueTask<IWikiUser?> GetUserAsync(ClaimsPrincipal? principal);
}
