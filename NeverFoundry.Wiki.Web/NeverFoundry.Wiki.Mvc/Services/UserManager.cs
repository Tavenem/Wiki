using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.Services
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class UserManager<TUser> : IUserManager where TUser : IdentityUser, IWikiUser
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<TUser> _userManager;

        public UserManager(Microsoft.AspNetCore.Identity.UserManager<TUser> userManager)
            => _userManager = userManager;

        /// <summary>
        /// Gets the user, if any, associated with the normalized value of the specified
        /// email address.
        /// </summary>
        /// <param name="email">The email address to return the user for.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation,
        /// the user, if any, associated with a normalized value of the specified email address.
        /// </returns>
        public async Task<IWikiUser?> FindByEmailAsync(string? email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }
            return await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user
        /// matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        public async Task<IWikiUser?> FindByIdAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            return await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified user name.
        /// </summary>
        /// <param name="userName">The user name to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user
        /// matching the specified <paramref name="userName"/> if it exists.
        /// </returns>
        public async Task<IWikiUser?> FindByNameAsync(string? userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }
            return await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of <see cref="Claim"/>s to be belonging to the specified user as an
        /// asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose claims to retrieve.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task{T}"/> that represents the result of the
        /// asynchronous query, a list of <see cref="Claim"/>s.
        /// </returns>
        public async Task<IList<Claim>> GetClaimsAsync(IWikiUser? user)
        {
            if (!(user is TUser tUser))
            {
                return new List<Claim>();
            }
            return await _userManager.GetClaimsAsync(tUser).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType
        /// claim in the <paramref name="principal"/> or <see langword="null"/>.
        /// </summary>
        /// <param name="principal">The principal which contains the user id claim.</param>
        /// <returns>
        /// The user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType claim in
        /// the <paramref name="principal"/> or <see langword="null"/>
        /// </returns>
        public async Task<IWikiUser?> GetUserAsync(ClaimsPrincipal? principal)
        {
            if (principal is null)
            {
                return null;
            }
            return await _userManager.GetUserAsync(principal).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a list of users from the user store who have the specified <paramref
        /// name="claim"/>.
        /// </summary>
        /// <param name="claim">The claim to look for.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> that represents the result of the asynchronous query, a list of
        /// <see cref="IWikiUser"/>s who have the specified <paramref name="claim"/>.
        /// </returns>
        public async Task<IList<IWikiUser>> GetUsersForClaimAsync(Claim? claim)
        {
            if (claim is null)
            {
                return new List<IWikiUser>();
            }
            return (IList<IWikiUser>)await _userManager.GetUsersForClaimAsync(claim).ConfigureAwait(false);
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
