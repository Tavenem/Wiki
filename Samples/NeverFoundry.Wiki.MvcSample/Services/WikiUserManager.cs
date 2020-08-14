using Microsoft.AspNetCore.Identity;
using NeverFoundry.Wiki.Web;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MvcSample.Services
{
    /// <summary>
    /// An implementation of <see cref="IWikiUserManager"/> with a <see cref="UserManager{TUser}"/>
    /// backing store.
    /// </summary>
    public class WikiUserManager<TUser> : IWikiUserManager where TUser : IdentityUser, IWikiUser
    {
        private readonly UserManager<TUser> _userManager;

        /// <summary>
        /// Initializes a new instance of <see cref="WikiUserManager{TUser}"/>.
        /// </summary>
        /// <param name="userManager">The <see
        /// cref="UserManager{TUser}"/> upon which this instance is
        /// based.</param>
        public WikiUserManager(UserManager<TUser> userManager)
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
        public async ValueTask<IWikiUser?> FindByEmailAsync(string? email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            return await AddClaimsAsync(user).ConfigureAwait(false);
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user
        /// matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        public async ValueTask<IWikiUser?> FindByIdAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            return await AddClaimsAsync(user).ConfigureAwait(false);
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified user name.
        /// </summary>
        /// <param name="userName">The user name to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user
        /// matching the specified <paramref name="userName"/> if it exists.
        /// </returns>
        public async ValueTask<IWikiUser?> FindByNameAsync(string? userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            return await AddClaimsAsync(user).ConfigureAwait(false);
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
        public async ValueTask<IWikiUser?> GetUserAsync(ClaimsPrincipal? principal)
        {
            if (principal is null)
            {
                return null;
            }
            var user = await _userManager.GetUserAsync(principal).ConfigureAwait(false);
            return await AddClaimsAsync(user).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a list of all wiki admin users.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}" /> that represents the result of the asynchronous query, a
        /// list of <see cref="IWikiUser" />s who have <see cref="IWikiUser.IsWikiAdmin" /> set to
        /// <see langword="true" />.
        /// </returns>
        public async ValueTask<IList<IWikiUser>> GetWikiAdminUsersAsync()
        {
            var users = await _userManager.GetUsersForClaimAsync(new Claim(WikiClaims.Claim_WikiAdmin, true.ToString(), ClaimValueTypes.Boolean)).ConfigureAwait(false);
            for (var i = 0; i < users.Count; i++)
            {
                var user = await AddClaimsAsync(users[i]).ConfigureAwait(false);
                if (user is not null)
                {
                    users[i] = user;
                }
            }
            return (IList<IWikiUser>)users;
        }

        private async ValueTask<TUser?> AddClaimsAsync(TUser? user)
        {
            if (user is null)
            {
                return null;
            }

            var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);

            var adminClaim = claims.FirstOrDefault(x => x.Type == WikiClaims.Claim_WikiAdmin);
            if (adminClaim is not null)
            {
                user.IsWikiAdmin = bool.TryParse(adminClaim.Value, out var value) && value;
            }

            var groupClaims = claims.Where(x => x.Type == WikiClaims.Claim_WikiGroup).ToList();
            if (groupClaims.Count > 0)
            {
                user.Groups = groupClaims.Select(x => x.Value).ToList();
            }

            return user;
        }
    }
}
