﻿using NeverFoundry.Wiki.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Simple.Services
{
    /// <summary>
    /// An implementation of <see cref="IWikiGroupManager"/> which never returns results.
    /// </summary>
    public class WikiGroupManager : IWikiGroupManager
    {
        /// <summary>
        /// Finds and returns a group, if any, which has the specified <paramref name="groupId" />.
        /// </summary>
        /// <param name="groupId">The group ID to search for.</param>
        /// <returns>
        /// The <see cref="ValueTask" /> that represents the asynchronous operation, containing the
        /// group matching the specified <paramref name="groupId" /> if it exists.
        /// </returns>
        public ValueTask<IWikiGroup?> FindByIdAsync(string? groupId)
            => new ValueTask<IWikiGroup?>((IWikiGroup?)null);

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
        public ValueTask<IWikiGroup?> FindByNameAsync(string? groupName)
            => new ValueTask<IWikiGroup?>((IWikiGroup?)null);

        /// <summary>
        /// Returns a list of all wiki users in the group with the given ID.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}" /> that represents the result of the asynchronous query, a
        /// list of <see cref="IWikiUser" />s whose <see cref="IWikiUser.Groups" /> list contains
        /// the given ID.
        /// </returns>
        public ValueTask<IList<IWikiUser>> GetUsersInGroupAsync(string? groupId)
            => new ValueTask<IList<IWikiUser>>(new List<IWikiUser>());

        /// <summary>
        /// Returns a list of all wiki users in the given <paramref name="group" />.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}" /> that represents the result of the asynchronous query, a
        /// list of <see cref="IWikiUser" />s whose <see cref="IWikiUser.Groups" /> list contains
        /// the given <paramref name="group" />'s ID.
        /// </returns>
        public ValueTask<IList<IWikiUser>> GetUsersInGroupAsync(IWikiGroup? group)
            => new ValueTask<IList<IWikiUser>>(new List<IWikiUser>());

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
        public ValueTask<bool> UserIsInGroup(string? groupId, string? userId)
            => new ValueTask<bool>(false);

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
            => new ValueTask<bool>(false);

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
            => new ValueTask<bool>(false);

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
            => new ValueTask<bool>(false);

        /// <summary>
        /// Determines if a user with the given ID is in any group with upload permission.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>
        /// The <see cref="ValueTask" /> that represents the asynchronous operation, containing <see
        /// langword="true" /> if a user with the given ID is in any group with upload permission,
        /// and <see langword="false" /> if no such user exists, or if the user does not belong to
        /// such a group.
        /// </returns>
        public ValueTask<bool> UserIsInUploadGroup(string? userId)
            => new ValueTask<bool>(false);

        /// <summary>
        /// Determines if the given <paramref name="user" /> is in any group with upload permission.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>
        /// The <see cref="ValueTask" /> that represents the asynchronous operation, containing <see
        /// langword="true" /> if the given <paramref name="user" /> is in any group with upload
        /// permission, and <see langword="false" /> if no such user exists, or if the user does not
        /// belong to such a group.
        /// </returns>
        public ValueTask<bool> UserIsInUploadGroup(IWikiUser? user)
            => new ValueTask<bool>(false);
    }
}
