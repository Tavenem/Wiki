using System.Security.Claims;
using Tavenem.DataStorage;

namespace Tavenem.Wiki.Services;

/// <summary>
/// A default user manager for <see cref="WikiUser"/>s, which keeps its data in an <see
/// cref="IDataStore"/>.
/// </summary>
/// <param name="dataStore">
/// The <see cref="IDataStore"/> to use.
/// </param>
public class WikiUserManager(IDataStore dataStore) : IWikiUserManager
{
    /// <summary>
    /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">The user ID to search for.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing the user
    /// matching the specified <paramref name="userId"/> if it exists.
    /// </returns>
    public async ValueTask<IWikiUser?> FindByIdAsync(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }
        return await dataStore.GetItemAsync(userId, WikiJsonSerializerContext.Default.WikiUser);
    }

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
    public async ValueTask<IWikiUser?> FindByNameAsync(string? userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return null;
        }
        var matches = await dataStore
            .Query(WikiJsonSerializerContext.Default.WikiUser)
            .Where(x => string.Equals(x.DisplayName, userName, StringComparison.Ordinal))
            .ToListAsync();
        return matches.Count == 1
            ? matches[0]
            : null;
    }

    /// <summary>
    /// Returns the user corresponding to the given <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="principal">A <see cref="ClaimsPrincipal"/>.</param>
    /// <returns>
    /// The user corresponding to given <see cref="ClaimsPrincipal"/>; or <see langword="null"/> if
    /// there is no such user.
    /// </returns>
    public async ValueTask<IWikiUser?> GetUserAsync(ClaimsPrincipal? principal)
    {
        if (principal is null)
        {
            return null;
        }

        var idClaim = principal.FindFirst("sub");
        idClaim ??= principal.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim is null)
        {
            return null;
        }

        return await FindByIdAsync(idClaim.Value);
    }
}
