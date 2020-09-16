using Microsoft.AspNetCore.Identity;
using NeverFoundry.Wiki.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Complete.Services
{
    /// <summary>
    /// An implementation of <see cref="IWikiGroupManager"/> with <see cref="UserManager{TUser}"/>
    /// and <see cref="DataStorage"/> backing stores.
    /// </summary>
    public class WikiGroupManager<TUser> : IWikiGroupManager where TUser : IdentityUser, IWikiUser
    {
        private readonly UserManager<TUser> _userManager;

        /// <summary>
        /// Initializes a new instance of <see cref="WikiGroupManager{TUser}"/>.
        /// </summary>
        /// <param name="userManager">The <see
        /// cref="UserManager{TUser}"/> upon which this instance is
        /// based.</param>
        public WikiGroupManager(UserManager<TUser> userManager)
            => _userManager = userManager;

        /// <summary>
        /// Finds and returns a group, if any, which has the specified <paramref name="groupId" />.
        /// </summary>
        /// <param name="groupId">The group ID to search for.</param>
        /// <returns>
        /// The <see cref="ValueTask" /> that represents the asynchronous operation, containing the
        /// group matching the specified <paramref name="groupId" /> if it exists.
        /// </returns>
        public async ValueTask<IWikiGroup?> FindByIdAsync(string? groupId)
            => await WikiConfig.DataStore.GetItemAsync<WikiGroup>(groupId).ConfigureAwait(false);

        /// <summary>
        /// <para>
        /// Finds and returns a group, if any, which has the specified group name.
        /// </para>
        /// <para>
        /// Returns <see langword="null" /> if there is more than one group with the specified name.
        /// In other words, this only returns a result if the given name has a unique match.
        /// </para>
        /// </summary>
        /// <param name="groupName">The group name to search for.</param>
        /// <returns>
        /// The <see cref="ValueTask" /> that represents the asynchronous operation, containing the
        /// group matching the specified <paramref name="groupName" /> if it exists.
        /// </returns>
        public async ValueTask<IWikiGroup?> FindByNameAsync(string? groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return null;
            }
            var list = await WikiConfig.DataStore.Query<WikiGroup>()
                .Where(x => x.GroupName == groupName)
                .Take(2)
                .ToListAsync()
                .ConfigureAwait(false);
            return list.Count == 1 ? list[0] : null;
        }

        /// <summary>
        /// Returns a list of all wiki users in the group with the given ID.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}" /> that represents the result of the asynchronous query, a
        /// list of <see cref="IWikiUser" />s whose <see cref="IWikiUser.Groups" /> list contains
        /// the given ID.
        /// </returns>
        public async ValueTask<IList<IWikiUser>> GetUsersInGroupAsync(string? groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return new List<IWikiUser>();
            }
            var users = await _userManager.GetUsersForClaimAsync(new Claim(WikiClaims.Claim_WikiGroup, groupId)).ConfigureAwait(false);
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

        /// <summary>
        /// Returns a list of all wiki users in the given <paramref name="group" />.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}" /> that represents the result of the asynchronous query, a
        /// list of <see cref="IWikiUser" />s whose <see cref="IWikiUser.Groups" /> list contains
        /// the given <paramref name="group" />'s ID.
        /// </returns>
        public ValueTask<IList<IWikiUser>> GetUsersInGroupAsync(IWikiGroup? group) => GetUsersInGroupAsync(group?.Id);

        /// <summary>
        /// Determines if a user with the given ID is in the group with the given ID.
        /// </summary>
        /// <param name="groupId">The group ID to search for.</param>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>
        /// The <see cref="ValueTask" /> that represents the asynchronous operation, containing <see
        /// langword="true" /> if a user with the given ID is in the group with the given ID, and
        /// <see langword="false" /> if no such group or user exists, or if the user does not belong
        /// to the group.
        /// </returns>
        public async ValueTask<bool> UserIsInGroup(string? groupId, string? userId)
        {
            if (string.IsNullOrEmpty(userId)
                || string.IsNullOrEmpty(groupId))
            {
                return false;
            }
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user is null)
            {
                return false;
            }
            var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            return claims.Any(x => x.Type == WikiClaims.Claim_WikiGroup && x.Value == groupId);
        }

        /// <summary>
        /// Determines if a user with the given ID is in the given <paramref name="group" />.
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>
        /// The <see cref="ValueTask" /> that represents the asynchronous operation, containing <see
        /// langword="true" /> if a user with the given ID is in the given <paramref name="group"
        /// />, and <see langword="false" /> if no such group or user exists, or if the user does
        /// not belong to the group.
        /// </returns>
        public ValueTask<bool> UserIsInGroup(IWikiGroup? group, string? userId)
            => UserIsInGroup(group?.Id, userId);

        /// <summary>
        /// Determines if the given <paramref name="user" /> is in the group with the given ID.
        /// </summary>
        /// <param name="groupId">The group ID to search for.</param>
        /// <param name="user">The user to check.</param>
        /// <returns>
        /// The <see cref="ValueTask" /> that represents the asynchronous operation, containing <see
        /// langword="true" /> if the given <paramref name="user" /> is in the group with the given
        /// ID, and <see langword="false" /> if no such group or user exists, or if the user does
        /// not belong to the group.
        /// </returns>
        public ValueTask<bool> UserIsInGroup(string? groupId, IWikiUser? user)
            => new ValueTask<bool>(!string.IsNullOrEmpty(groupId) && user?.Groups?.Contains(groupId) == true);

        /// <summary>
        /// Determines if the given <paramref name="user" /> is in the given <paramref name="group"
        /// />.
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <param name="user">The user to check.</param>
        /// <returns>
        /// The <see cref="ValueTask" /> that represents the asynchronous operation, containing <see
        /// langword="true" /> if the given <paramref name="user" /> is in the given <paramref
        /// name="group" />, and <see langword="false" /> if no such group or user exists, or if the
        /// user does not belong to the group.
        /// </returns>
        public ValueTask<bool> UserIsInGroup(IWikiGroup? group, IWikiUser? user)
            => new ValueTask<bool>(group is not null && user?.Groups?.Contains(group.Id) == true);

        /// <summary>
        /// Determines the maximum upload limit of a user with the given ID.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>
        /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing the
        /// maximum upload limit of the user with the given ID (note that any negative value is
        /// "greater" than any positive value, since it indicates no limit). Returns zero if no such
        /// user exists.
        /// </returns>
        public async ValueTask<int> UserMaxUploadLimit(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return 0;
            }
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user is null)
            {
                return 0;
            }
            var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            var max = 0;
            foreach (var groupId in claims
                .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                .Select(x => x.Value))
            {
                var group = await WikiConfig.DataStore.GetItemAsync<WikiGroup>(groupId).ConfigureAwait(false);
                if (group is not null)
                {
                    if (group.UploadLimit < 0)
                    {
                        return group.UploadLimit;
                    }
                    max = Math.Max(max, group.UploadLimit);
                }
            }
            return max;
        }

        /// <summary>
        /// Determines if the given <paramref name="user"/> is in any group with upload permission.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>
        /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing the
        /// maximum upload limit of the given <paramref name="user"/> (note that any negative value
        /// is "greater" than any positive value, since it indicates no limit). Returns zero if no
        /// such user exists.
        /// </returns>
        public ValueTask<int> UserMaxUploadLimit(IWikiUser? user)
            => UserMaxUploadLimit(user?.Id);

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
