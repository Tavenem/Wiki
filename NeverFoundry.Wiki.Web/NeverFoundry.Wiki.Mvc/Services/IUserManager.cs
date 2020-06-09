using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.Services
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public interface IUserManager
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
        /// Gets a list of <see cref="Claim"/>s to be belonging to the specified user as an
        /// asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose claims to retrieve.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task{T}"/> that represents the result of the
        /// asynchronous query, a list of <see cref="Claim"/>s.
        /// </returns>
        Task<IList<Claim>> GetClaimsAsync(IWikiUser? user);

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
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
