using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Web
{
    /// <summary>
    /// A user manager interface for <see cref="IWikiUser"/>s.
    /// </summary>
    public interface IWikiUserManager
    {
        /// <summary>
        /// Gets the user, if any, associated with the normalized value of the specified
        /// email address.
        /// </summary>
        /// <param name="email">The email address to return the user for.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation,
        /// the user, if any, associated with a normalized value of the specified email address.
        /// </returns>
        Task<IWikiUser?> FindByEmailAsync(string? email);

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user
        /// matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        Task<IWikiUser?> FindByIdAsync(string? userId);

        /// <summary>
        /// Finds and returns a user, if any, who has the specified user name.
        /// </summary>
        /// <param name="userName">The user name to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user
        /// matching the specified <paramref name="userName"/> if it exists.
        /// </returns>
        Task<IWikiUser?> FindByNameAsync(string? userName);

        /// <summary>
        /// Returns the user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType
        /// claim in the <paramref name="principal"/> or <see langword="null"/>.
        /// </summary>
        /// <param name="principal">The principal which contains the user id claim.</param>
        /// <returns>
        /// The user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType claim in
        /// the <paramref name="principal"/> or <see langword="null"/>
        /// </returns>
        Task<IWikiUser?> GetUserAsync(ClaimsPrincipal? principal);

        /// <summary>
        /// Returns a list of all wiki users in the group with the given ID.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}"/> that represents the result of the asynchronous query, a list of
        /// <see cref="IWikiUser"/>s whose <see cref="IWikiUser.Groups"/> list contains the given ID.
        /// </returns>
        Task<IList<IWikiUser>> GetUsersInGroupAsync(string? groupId);

        /// <summary>
        /// Returns a list of all wiki users in the given <paramref name="group"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}"/> that represents the result of the asynchronous query, a list of
        /// <see cref="IWikiUser"/>s whose <see cref="IWikiUser.Groups"/> list contains the given
        /// <paramref name="group"/>'s ID.
        /// </returns>
        Task<IList<IWikiUser>> GetUsersInGroupAsync(IWikiGroup? group);

        /// <summary>
        /// Returns a list of all wiki admin users.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}"/> that represents the result of the asynchronous query, a list of
        /// <see cref="IWikiUser"/>s who have <see cref="IWikiUser.IsWikiAdmin"/> set to <see
        /// langword="true"/>.
        /// </returns>
        Task<IList<IWikiUser>> GetWikiAdminUsersAsync();
    }
}
