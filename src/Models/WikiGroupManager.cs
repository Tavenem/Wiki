﻿using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// A default group manager for <see cref="WikiGroup"/>s, which keeps its data in an <see
/// cref="IDataStore"/>.
/// </summary>
public class WikiGroupManager : IWikiGroupManager
{
    private readonly IDataStore _dataStore;
    private readonly IWikiUserManager _userManager;

    /// <summary>
    /// Constructs a new instance of <see cref="WikiGroupManager"/>.
    /// </summary>
    /// <param name="dataStore">
    /// The <see cref="IDataStore"/> to use.
    /// </param>
    /// <param name="userManager">
    /// An <see cref="IWikiUserManager"/> instance.
    /// </param>
    public WikiGroupManager(
        IDataStore dataStore,
        IWikiUserManager userManager)
    {
        _dataStore = dataStore;
        _userManager = userManager;
    }

    /// <summary>
    /// Finds and returns a group, if any, which has the specified <paramref name="groupId"/>.
    /// </summary>
    /// <param name="groupId">The group ID to search for.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing the
    /// group matching the specified <paramref name="groupId"/> if it exists.
    /// </returns>
    public async ValueTask<IWikiGroup?> FindByIdAsync(string? groupId)
    {
        if (string.IsNullOrWhiteSpace(groupId))
        {
            return null;
        }
        return await _dataStore.GetItemAsync<WikiGroup>(groupId);
    }

    /// <summary>
    /// <para>
    /// Finds and returns a group, if any, which has the specified group name.
    /// </para>
    /// <para>
    /// Returns <see langword="null"/> if there is more than one group with the specified name.
    /// In other words, this only returns a result if the given name has a unique match.
    /// </para>
    /// </summary>
    /// <param name="groupName">The group name to search for.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing the
    /// group matching the specified <paramref name="groupName"/> if it exists.
    /// </returns>
    public async ValueTask<IWikiGroup?> FindByNameAsync(string? groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return null;
        }
        var matches = await _dataStore.Query<WikiGroup>()
            .Where(x => string.Equals(x.DisplayName, groupName, StringComparison.Ordinal))
            .ToListAsync();
        return matches.Count == 1
            ? matches[0]
            : null;
    }

    /// <summary>
    /// Returns the wiki user who is the owner of the group with the given ID.
    /// </summary>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> that represents the result of the asynchronous query, an <see
    /// cref="IWikiUser"/> whose <see cref="IWikiOwner.Id"/> matches the <see
    /// cref="IWikiGroup.OwnerId"/> of the group with the given ID; or <see langword="null"/> if no
    /// such group or user exists.
    /// </returns>
    public async ValueTask<IWikiUser?> GetGroupOwnerAsync(string? groupId)
    {
        if (string.IsNullOrWhiteSpace(groupId))
        {
            return null;
        }
        var group = await FindByIdAsync(groupId);
        if (string.IsNullOrWhiteSpace(group?.OwnerId))
        {
            return null;
        }
        return await _userManager.FindByIdAsync(group.OwnerId);
    }

    /// <summary>
    /// Returns the wiki user who is the owner of the given <paramref name="group"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> that represents the result of the asynchronous query, an <see
    /// cref="IWikiUser"/> whose <see cref="IWikiOwner.Id"/> matches the <see
    /// cref="IWikiGroup.OwnerId"/> of the given <paramref name="group"/>; or <see langword="null"/>
    /// if no such group or user exists.
    /// </returns>
    public async ValueTask<IWikiUser?> GetGroupOwnerAsync(IWikiGroup? group)
        => string.IsNullOrWhiteSpace(group?.OwnerId)
        ? null
        : await _userManager.FindByIdAsync(group.OwnerId);

    /// <summary>
    /// Returns the ID of the wiki user who is the owner of the group with the given ID.
    /// </summary>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> that represents the result of the asynchronous query, a <see
    /// cref="string"/> containing the <see cref="IWikiGroup.OwnerId"/> of the group with the given
    /// ID; or <see langword="null"/> if no such group exists.
    /// </returns>
    public async ValueTask<string?> GetGroupOwnerIdAsync(string? groupId)
    {
        if (string.IsNullOrWhiteSpace(groupId))
        {
            return null;
        }
        var group = await FindByIdAsync(groupId);
        return group?.OwnerId;
    }

    /// <summary>
    /// Returns a list of all wiki users in the group with the given ID.
    /// </summary>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> that represents the result of the asynchronous query, a list of
    /// <see cref="IWikiUser"/>s whose <see cref="IWikiUser.Groups"/> list contains the given ID.
    /// </returns>
    public async ValueTask<IReadOnlyList<IWikiUser>> GetUsersInGroupAsync(string? groupId)
    {
        if (string.IsNullOrWhiteSpace(groupId))
        {
            return new List<IWikiUser>();
        }
        return await _dataStore.Query<WikiUser>()
            .Where(x => x.Groups != null && x.Groups.Contains(groupId))
            .ToListAsync();
    }

    /// <summary>
    /// Returns a list of all wiki users in the given <paramref name="group"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> that represents the result of the asynchronous query, a list of
    /// <see cref="IWikiUser"/>s whose <see cref="IWikiUser.Groups"/> list contains the given
    /// <paramref name="group"/>'s ID.
    /// </returns>
    public async ValueTask<IReadOnlyList<IWikiUser>> GetUsersInGroupAsync(IWikiGroup? group)
    {
        if (group is null)
        {
            return new List<IWikiUser>();
        }
        return await _dataStore.Query<WikiUser>()
            .Where(x => x.Groups != null && x.Groups.Contains(group.Id))
            .ToListAsync();
    }

    /// <summary>
    /// Determines if the given <paramref name="user"/> is the owner of the group with the given ID.
    /// </summary>
    /// <param name="groupId">The group ID to search for.</param>
    /// <param name="user">The user to check.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing <see
    /// langword="true"/> if the given <paramref name="user"/> is the owner of the group with the
    /// given ID, and <see langword="false"/> if no such group or user exists, or if the user is not
    /// the owner of the group.
    /// </returns>
    public async ValueTask<bool> UserIsGroupOwner(string? groupId, IWikiUser? user)
    {
        if (user is null
            || string.IsNullOrWhiteSpace(groupId))
        {
            return false;
        }
        var group = await FindByIdAsync(groupId);
        if (group is null)
        {
            return false;
        }
        return string.Equals(group.OwnerId, user.Id, StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines if the given <paramref name="user"/> is the owner of the given <paramref
    /// name="group"/>.
    /// </summary>
    /// <param name="group">The group to check.</param>
    /// <param name="user">The user to check.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing <see
    /// langword="true"/> if the given <paramref name="user"/> is the owner of the given <paramref
    /// name="group"/>, and <see langword="false"/> if no such group or user exists, or if the user
    /// is not the owner of the group.
    /// </returns>
    public ValueTask<bool> UserIsGroupOwner(IWikiGroup? group, IWikiUser? user)
    {
        if (group is null
            || user is null
            || string.IsNullOrWhiteSpace(group.OwnerId))
        {
            return new ValueTask<bool>(false);
        }
        return new ValueTask<bool>(string.Equals(group.OwnerId, user.Id, StringComparison.Ordinal));
    }

    /// <summary>
    /// Determines if a user with the given ID is in the group with the given ID.
    /// </summary>
    /// <param name="groupId">The group ID to search for.</param>
    /// <param name="userId">The user ID to search for.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing <see
    /// langword="true"/> if a user with the given ID is in the group with the given ID, and
    /// <see langword="false"/> if no such group or user exists, or if the user does not belong
    /// to the group.
    /// </returns>
    public async ValueTask<bool> UserIsInGroup(string? groupId, string? userId)
    {
        if (string.IsNullOrWhiteSpace(groupId)
            || string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }
        return user.Groups?.Contains(groupId) ?? false;
    }

    /// <summary>
    /// Determines if a user with the given ID is in the given <paramref name="group"/>.
    /// </summary>
    /// <param name="group">The group to check.</param>
    /// <param name="userId">The user ID to search for.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing <see
    /// langword="true"/> if a user with the given ID is in the given <paramref name="group"/>,
    /// and <see langword="false"/> if no such group or user exists, or if the user does not
    /// belong to the group.
    /// </returns>
    public async ValueTask<bool> UserIsInGroup(IWikiGroup? group, string? userId)
    {
        if (group is null
            || string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }
        return user.Groups?.Contains(group.Id) ?? false;
    }

    /// <summary>
    /// Determines if the given <paramref name="user"/> is in the group with the given ID.
    /// </summary>
    /// <param name="groupId">The group ID to search for.</param>
    /// <param name="user">The user to check.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing <see
    /// langword="true"/> if the given <paramref name="user"/> is in the group with the given
    /// ID, and <see langword="false"/> if no such group or user exists, or if the user does not
    /// belong to the group.
    /// </returns>
    public ValueTask<bool> UserIsInGroup(string? groupId, IWikiUser? user)
        => new(!string.IsNullOrWhiteSpace(groupId)
        && user?.Groups?.Contains(groupId) == true);

    /// <summary>
    /// Determines if the given <paramref name="user"/> is in the given <paramref
    /// name="group"/>.
    /// </summary>
    /// <param name="group">The group to check.</param>
    /// <param name="user">The user to check.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing <see
    /// langword="true"/> if the given <paramref name="user"/> is in the given <paramref
    /// name="group"/>, and <see langword="false"/> if no such group or user exists, or if the
    /// user does not belong to the group.
    /// </returns>
    public ValueTask<bool> UserIsInGroup(IWikiGroup? group, IWikiUser? user)
        => new(group is not null
        && user?.Groups?.Contains(group.Id) == true);

    /// <summary>
    /// Determines the maximum upload limit of a user with the given ID.
    /// </summary>
    /// <param name="userId">The user ID to search for.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing the
    /// maximum upload limit of the user with the given ID (note that any negative value is
    /// "greater" than any positive value, since it indicates no limit). Returns zero if no such
    /// user exists, or the user is deleted or disabled.
    /// </returns>
    public async ValueTask<int> UserMaxUploadLimit(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return 0;
        }
        var user = await _userManager.FindByIdAsync(userId);
        return await UserMaxUploadLimit(user);
    }

    /// <summary>
    /// Determines if the given <paramref name="user"/> is in any group with upload permission.
    /// </summary>
    /// <param name="user">The user to check.</param>
    /// <returns>
    /// The <see cref="ValueTask"/> that represents the asynchronous operation, containing the
    /// maximum upload limit of the given <paramref name="user"/> (note that any negative value
    /// is "greater" than any positive value, since it indicates no limit). Returns zero if no
    /// such user exists, or the user is deleted or disabled.
    /// </returns>
    public async ValueTask<int> UserMaxUploadLimit(IWikiUser? user)
    {
        if (user is null
            || user.IsDeleted
            || user.IsDisabled)
        {
            return 0;
        }
        if (user.UploadLimit < 0
            || user.Groups is null)
        {
            return -1;
        }
        var max = user.UploadLimit;
        foreach (var groupId in user.Groups)
        {
            var group = await FindByIdAsync(groupId);
            if (group is null)
            {
                continue;
            }
            if (group.UploadLimit < 0)
            {
                return -1;
            }
            if (group.UploadLimit > max)
            {
                max = group.UploadLimit;
            }
        }
        return max;
    }
}
