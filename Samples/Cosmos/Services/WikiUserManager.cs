using NeverFoundry.Wiki.Web;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Cosmos.Services
{
    /// <summary>
    /// An implementation of <see cref="IWikiUserManager"/> which always returns a static user.
    /// </summary>
    public class WikiUserManager : IWikiUserManager
    {
        public const string UserId = "A478AF94-44AF-4F21-AD65-71B77B9A569A";
        private static WikiUser _User = new WikiUser("User")
        {
            Email = "example@example.com",
            EmailConfirmed = true,
            Id = UserId,
            IsWikiAdmin = true,
        };

        /// <summary>
        /// Gets the user, if any, associated with the normalized value of the specified
        /// email address.
        /// </summary>
        /// <param name="email">The email address to return the user for.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation,
        /// the user, if any, associated with a normalized value of the specified email address.
        /// </returns>
        public ValueTask<IWikiUser?> FindByEmailAsync(string? email)
            => new ValueTask<IWikiUser?>(string.IsNullOrEmpty(email) ? null : _User);

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user
        /// matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        public ValueTask<IWikiUser?> FindByIdAsync(string? userId)
            => new ValueTask<IWikiUser?>(string.IsNullOrEmpty(userId) ? null : _User);

        /// <summary>
        /// Finds and returns a user, if any, who has the specified user name.
        /// </summary>
        /// <param name="userName">The user name to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user
        /// matching the specified <paramref name="userName"/> if it exists.
        /// </returns>
        public ValueTask<IWikiUser?> FindByNameAsync(string? userName)
            => new ValueTask<IWikiUser?>(string.IsNullOrEmpty(userName) ? null : _User);

        /// <summary>
        /// Returns the user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType
        /// claim in the <paramref name="principal"/> or <see langword="null"/>.
        /// </summary>
        /// <param name="principal">The principal which contains the user id claim.</param>
        /// <returns>
        /// The user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType claim in
        /// the <paramref name="principal"/> or <see langword="null"/>
        /// </returns>
        public ValueTask<IWikiUser?> GetUserAsync(ClaimsPrincipal? principal)
            => new ValueTask<IWikiUser?>(_User);

        /// <summary>
        /// Returns a list of all wiki admin users.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}" /> that represents the result of the asynchronous query, a
        /// list of <see cref="IWikiUser" />s who have <see cref="IWikiUser.IsWikiAdmin" /> set to
        /// <see langword="true" />.
        /// </returns>
        public ValueTask<IList<IWikiUser>> GetWikiAdminUsersAsync()
            => new ValueTask<IList<IWikiUser>>(new List<IWikiUser> { _User });
    }
}
